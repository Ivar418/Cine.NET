using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SharedLibrary.DTOs.Responses;
using System.Text.Json;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.DTOs.Responses.TMDB;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SharedLibrary.DTOs.Models;


namespace API.Infrastructure.Database
{
    using SharedLibrary.Domain.Entities;
    using System;

    public static class DbSeeder
    {
        public static async Task SeedAsync(ApiDbContext db, IMovieService movieService, IShowingService showingService,
            ITicketService ticketService, IPricingService pricingService, IAuditoriumService auditoriumService,
            ILocalMailService localMailService)
        {
            var movieEntities = new List<Movie>();
            if (!await db.Users.AnyAsync())
            {
                db.Users.AddRange(
                    new User("Admin"),
                    new User("TestUser"),
                    new User("John Doe"),
                    new User("Jane Smith")
                );
            }

            if (!await db.Movies.AnyAsync())
            {
                // 285 = Pirates of the Caribbean: At World's End
                // 83533 = Avatar: Fire and Ash
                // 1272837 = 28 Years Later: The Bone Temple
                // 1242898 = Predator: Badlands
                var MovieIdList = new List<int> { 285, 83533, 1272837, 1242898 };
                foreach (var id in MovieIdList)
                {
                    var movie = await movieService.AddMovieAsyncForEachSpecifiedLanguage(tmdbId: id);
                }

                // Fill the genres table with all genres from TMDB for all specified languages (en, nl)
                await movieService.FetchAllGenresForAllSpecifiedLanguagesAndSaveToDb();
            }

            if (!await db.PaymentMethods.AnyAsync())
            {
                db.PaymentMethods.AddRange(
                    new PaymentMethod { Code = "PIN", DisplayName = "PIN" },
                    new PaymentMethod { Code = "IDEAL", DisplayName = "iDEAL" },
                    new PaymentMethod { Code = "CREDITCARD", DisplayName = "Credit Card" }
                );
                await db.SaveChangesAsync();
            }

            if (!await db.TicketTypes.AnyAsync())
            {
                db.TicketTypes.AddRange(
                    new TicketType { Name = "Adult", Discount = 0.00m },
                    new TicketType { Name = "Child", Discount = 1.50m, },
                    new TicketType { Name = "Student", Discount = 1.50m },
                    new TicketType { Name = "Senior", Discount = 1.50m }
                );
            }

            if (!await db.PricingConfigs.AnyAsync())
            {
                db.PricingConfigs.AddRange(
                    new PricingConfig { Key = "BasePrice", Value = 8.50m },
                    new PricingConfig { Key = "LongMoviePrice", Value = 9.00m },
                    new PricingConfig { Key = "ThreeDSurcharge", Value = 2.50m }
                );
            }

            // For future use when we want to add more pricing options, but for now we can just calculate them on the fly in the API

            // if (!await db.PricingOptions.AnyAsync())
            // {
            //     db.PricingOptions.AddRange(
            //         new PricingOption { Name = "None", PriceModifier = 0.00m },
            //         new PricingOption { Name = "Popcorn", PriceModifier = 4.50m },
            //         new PricingOption { Name = "Nachos", PriceModifier = 5.00m },
            //         new PricingOption { Name = "VIPSeat", PriceModifier = 3.00m }
            //     );
            // }

            // AUDITORIUMS
            if (!await db.Auditoriums.AnyAsync())
            {
                var auditoriumsRequest = new List<CreateAuditoriumRequest>
                {
                    new CreateAuditoriumRequest("Zaal 1", new List<RowConfig>
                    {
                        new RowConfig(10, 1),
                        new RowConfig(11, 2),
                        new RowConfig(12, 3)
                    }),
                    new CreateAuditoriumRequest("Zaal 2", new List<RowConfig>
                    {
                        new RowConfig(13, 0),
                        new RowConfig(14, 1),
                        new RowConfig(15, 2)
                    }),
                    new CreateAuditoriumRequest("Zaal 3", new List<RowConfig>
                    {
                        new RowConfig(16, 0),
                        new RowConfig(17, 1),
                        new RowConfig(18, 2)
                    }),
                    new CreateAuditoriumRequest("Zaal 4", new List<RowConfig>
                    {
                        new RowConfig(19, 0),
                        new RowConfig(20, 1),
                        new RowConfig(21, 2)
                    }),
                    new CreateAuditoriumRequest("Zaal 5", new List<RowConfig>
                    {
                        new RowConfig(22, 10),
                        new RowConfig(23, 23),
                        new RowConfig(24, 11)
                    }),
                    new CreateAuditoriumRequest("Zaal 6", new List<RowConfig>
                    {
                        new RowConfig(25, 0),
                        new RowConfig(30, 1),
                        new RowConfig(40, 2)
                    }),
                };
                foreach (var request in auditoriumsRequest)
                {
                    await auditoriumService.AddAuditoriumAsync(request);
                }
            }


            if (!await db.Showings.AnyAsync())
            {
                var movies = movieService.GetMoviesAsync("nl").Result.Value?.ToList();
                var auditoriums = auditoriumService.GetAuditoriumsAsync().Result.Value?.ToList();

                var showings = new List<Showing>();
                var start = DateTimeOffset.UtcNow.Date.AddHours(18); // 18:00 start

                for (int i = 0; i < movies.Count; i++)
                {
                    showings.Add(new Showing
                    {
                        MovieId = movies[i].Id,
                        AuditoriumId = auditoriums[i % auditoriums.Count].Id,
                        StartsAt = start.AddHours(i * 2), // elke 2 uur
                        IsThreeD = (i % 2 == 0), // om en om 3D
                        AuditoriumLayoutSnapshot =
                            auditoriums[i].RowConfigJson // Sla de auditorium layout op als JSON string in de showing
                    });
                }

                db.Showings.AddRange(showings);
                await db.SaveChangesAsync(); // commit showings first so dummy order can reference one
            }

            // Dummy order for API testing when no orders exist
            if (!await db.Orders.AnyAsync())
            {
                var showing = await db.Showings.OrderBy(s => s.Id).FirstOrDefaultAsync();
                if (showing != null)
                {
                    var ticket = new Ticket
                    {
                        ShowingId = showing.Id,
                        ShowDateTimeUtc = showing.StartsAt.UtcDateTime.ToString("O"),
                        SeatNumber = "A1",
                        Price = 9.50m,
                        TicketType = "Adult",
                        PaymentStatus = "Pending",
                        QrIsActive = false
                    };
                    await db.Tickets.AddAsync(ticket);
                    await db.SaveChangesAsync();

                    var order = new Order
                    {
                        OrderCode = "DUMMYORDER001",
                        CreatedAtUtc = DateTime.UtcNow,
                        TotalAmount = ticket.Price,
                        OrderType = "Reservation",
                        PaymentStatus = "Pending",
                        PaymentMethod = "IDEAL",
                        IsPrinted = false,
                        OrderTickets = new List<OrderTicket>
                        {
                            new OrderTicket { TicketId = ticket.Id, Ticket = ticket }
                        }
                    };

                    await db.Orders.AddAsync(order);
                    await db.SaveChangesAsync();
                }
            }

            if (!await db.Tickets.AnyAsync())
            {
                await ticketService.CreateTicketAsync(new Ticket
                {
                    ShowingId = 1,
                    ShowDateTimeUtc = DateTimeOffset.UtcNow.Date.AddHours(18).ToString("O"),
                    SeatNumber = "A1",
                    TicketType = "Adult",
                    Price = 8.50m
                });
            }
            if (!await db.EmailSubscriptions.AnyAsync())
            {
                await localMailService.AddAsync("TheBeeKeerIsAmazing@Badazz.yow");
                await localMailService.AddAsync("Batman@adjlaskjd.nl");
                var textPart = new TextPart("plain")
                {
                    Text = @" Hello subscribers!,
                    
This is a test email to confirm that the subscription system is working correctly. Thank you for subscribing to our newsletter!

Groetjessssss,

CineNet."
                };
                await localMailService.SendEmailToSubscribersAsync(textPart, "CineNet", "Kom nu kijken!!");
            }

            await db.SaveChangesAsync();
        }
    }
}
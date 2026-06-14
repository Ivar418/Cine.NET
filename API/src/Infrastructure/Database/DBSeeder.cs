using System.Net.Sockets;
using API.Domain.Model;
using API.Services.Interfaces;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SharedLibrary.Domain.Entities.Enums;
using SharedLibrary.DTOs.Models;


namespace API.Infrastructure.Database {
    using SharedLibrary.Domain.Entities;
    using System;

    public static class DbSeeder {
        public static async Task SeedAsync(ApiDbContext db, IMovieService movieService, IShowingService showingService,
            ITicketService ticketService, IPricingService pricingService, IAuditoriumService auditoriumService,
            ILocalMailService localMailService, IAuthService authService) {
            var movieEntities = new List<Movie>();

            if (!await db.Movies.AnyAsync()) {
                // 285 = Pirates of the Caribbean: At World's End
                // 83533 = Avatar: Fire and Ash
                // 1272837 = 28 Years Later: The Bone Temple
                // 1242898 = Predator: Badlands
                var movieIdList = new List<int> {
                    // Existing
                    285, 83533, 1272837, 1242898,

                    // Action
                    76341, 284053, 299534,

                    // Comedy
                    505600, 93456,

                    // Drama
                    13, 497, 238,

                    // Animation (kids)
                    508943, 9806, 12,

                    // Family (kids)
                    11544, 672, 585,

                    // Sci-Fi
                    157336, 603, 11,

                    // Horror
                    694, 419430, 138843,

                    // Romance
                    597, 19995, 398818,

                    // NL gesproken
                    21872, 5497
                };
                foreach (var id in movieIdList) {
                    await movieService.AddMovieAsyncForEachSpecifiedLanguage(tmdbId: id);
                }

                // Fill the genres table with all genres from TMDB for all specified languages (en, nl)
                await movieService.FetchAllGenresForAllSpecifiedLanguagesAndSaveToDb();
            }

            if (!await db.TicketTypes.AnyAsync()) {
                db.TicketTypes.AddRange(
                    new TicketType { Name = "Adult", Discount = 0.00m },
                    new TicketType { Name = "Child", Discount = 1.50m, },
                    new TicketType { Name = "Student", Discount = 1.50m },
                    new TicketType { Name = "Senior", Discount = 1.50m }
                );
            }

            if (!await db.PricingConfigs.AnyAsync()) {
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
            if (!await db.Auditoriums.AnyAsync()) {
                var auditoriumsRequest = new List<CreateAuditoriumRequest> {
                    new CreateAuditoriumRequest("Zaal 1", new List<RowConfig> {
                        new RowConfig(15, 2),
                        new RowConfig(15, 0),
                        new RowConfig(15, 2),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 4)
                    }),
                    new CreateAuditoriumRequest("Zaal 2", new List<RowConfig> {
                        new RowConfig(15, 2),
                        new RowConfig(15, 0),
                        new RowConfig(15, 2),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 4)
                    }),
                    new CreateAuditoriumRequest("Zaal 3", new List<RowConfig> {
                        new RowConfig(15, 2),
                        new RowConfig(15, 0),
                        new RowConfig(15, 2),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(15, 4)
                    }),
                    new CreateAuditoriumRequest("Zaal 4", new List<RowConfig> {
                        new RowConfig(10, 0),
                        new RowConfig(10, 1),
                        new RowConfig(10, 2),
                        new RowConfig(10, 0),
                        new RowConfig(10, 1),
                        new RowConfig(10, 2)
                    }),
                    new CreateAuditoriumRequest("Zaal 5", new List<RowConfig> {
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(10, 0),
                        new RowConfig(10, 0)
                    }),
                    new CreateAuditoriumRequest("Zaal 6", new List<RowConfig> {
                        new RowConfig(15, 0),
                        new RowConfig(15, 0),
                        new RowConfig(10, 0),
                        new RowConfig(10, 0)
                    }),
                };
                foreach (var request in auditoriumsRequest) {
                    await auditoriumService.AddAuditoriumAsync(request);
                }
            }

            // Seed initial showings once; the background service will append one new day at a time.
            await EnsureInitialShowingsAsync(db);

            // Dummy order for API testing when no orders exist
            if (!await db.Orders.AnyAsync()) {
                var showing = await db.Showings.OrderBy(s => s.Id).FirstOrDefaultAsync();
                if (showing != null) {
                    var ticket = new Ticket {
                        ShowingId = showing.Id,
                        ShowDateTimeUtc = showing.StartsAt.UtcDateTime.ToString("O"),
                        SeatNumber = "A1",
                        Price = 9.50m,
                        TicketType = "Adult",
                        PaymentStatus = PaymentStatuses.Pending,
                        QrIsActive = false
                    };
                    await db.Tickets.AddAsync(ticket);
                    await db.SaveChangesAsync();

                    var order = new Order {
                        OrderCode = "DUMMYORDER001",
                        CreatedAtUtc = DateTime.UtcNow,
                        TotalAmount = ticket.Price,
                        OrderType = OrderTypes.Reservation,
                        PaymentStatuses = PaymentStatuses.Pending,
                        PaymentMethod = PaymentMethods.iDEAL,
                        IsPrinted = false,
                        OrderTickets = new List<OrderTicket> {
                            new OrderTicket { TicketId = ticket.Id, Ticket = ticket }
                        }
                    };

                    await db.Orders.AddAsync(order);
                    await db.SaveChangesAsync();
                }
            }

            if (!await db.Tickets.AnyAsync()) {
                await ticketService.CreateTicketAsync(new Ticket {
                    ShowingId = 1,
                    ShowDateTimeUtc = DateTimeOffset.UtcNow.Date.AddHours(18).ToString("O"),
                    SeatNumber = "A1",
                    TicketType = "Adult",
                    Price = 8.50m
                });
            }

            if (!await db.Arrangements.AnyAsync()) {
                var arr1 = new Arrangement {
                    Name = "Movie Deal - Popcorn & Cola",
                    Price = 12.00m,
                    IsActive = true
                };

                var arr2 = new Arrangement {
                    Name = "Movie Deal - M&M's & Fanta",
                    Price = 12.00m,
                    IsActive = true
                };

                db.Arrangements.AddRange(arr1, arr2);
                await db.SaveChangesAsync();
            }

            if (!await db.ArrangementItems.AnyAsync()) {
                var arr1 = await db.Arrangements.FirstAsync(a => a.Name.Contains("Popcorn"));
                var arr2 = await db.Arrangements.FirstAsync(a => a.Name.Contains("M&M"));

                db.ArrangementItems.AddRange(
                    new ArrangementItem {
                        ArrangementId = arr1.Id,
                        Type = ArrangementItemType.Ticket,
                        Name = "Ticket",
                        Quantity = 1
                    },
                    new ArrangementItem {
                        ArrangementId = arr1.Id,
                        Type = ArrangementItemType.Food,
                        Name = "Popcorn",
                        Quantity = 1
                    },
                    new ArrangementItem {
                        ArrangementId = arr1.Id,
                        Type = ArrangementItemType.Drink,
                        Name = "Cola",
                        Quantity = 1
                    },
                    new ArrangementItem {
                        ArrangementId = arr2.Id,
                        Type = ArrangementItemType.Ticket,
                        Name = "Ticket",
                        Quantity = 1
                    },
                    new ArrangementItem {
                        ArrangementId = arr2.Id,
                        Type = ArrangementItemType.Food,
                        Name = "M&M's",
                        Quantity = 1
                    },
                    new ArrangementItem {
                        ArrangementId = arr2.Id,
                        Type = ArrangementItemType.Drink,
                        Name = "Fanta",
                        Quantity = 1
                    }
                );

                await db.SaveChangesAsync();
            }


            if (!await db.EmailSubscriptions.AnyAsync()) {
                await localMailService.AddAsync("TheBeeKeerIsAmazing@Badazz.yow");
                await localMailService.AddAsync("Batman@adjlaskjd.nl");
                var textPart = new TextPart("plain") {
                    Text = @" Hello subscribers!,
                    
This is a test email to confirm that the subscription system is working correctly. Thank you for subscribing to our newsletter!

Groetjessssss,

CineNet."
                };
                    await localMailService.SendEmailToSubscribersAsync(textPart, "CineNet", "Kom nu kijken!!");

            }

            if (!await db.Users.AnyAsync()) {
                db.Users.AddRange(
                    new User(
                        userName: "admin",
                        firstName: "System",
                        lastName: "Administrator",
                        email: "admin@example.com"
                    ),
                    new User(
                        userName: "testuser",
                        firstName: "Test",
                        lastName: "User",
                        email: "testuser@example.com"
                    ),
                    new User(
                        userName: "johndoe",
                        firstName: "John",
                        lastName: "Doe",
                        email: "john.doe@example.com"
                    ),
                    new User(
                        userName: "janesmith",
                        firstName: "Jane",
                        lastName: "Smith",
                        email: "jane.smith@example.com"
                    )
                );
                db.SaveChanges();
                await foreach (var user in db.Users) {
                    if (user.UserName == "admin") {
                        var adminPassword = Env.GetString("ADMIN_PASSWORD");
                        if (string.IsNullOrWhiteSpace(adminPassword)) continue;

                        await db.AddAsync(new UserCredential(
                            userId: user.Id,
                            passwordHash: authService.PasswordHasher(adminPassword)
                        ));
                        continue;
                    }

                    var password =
                        Random.Shared.GetString(
                            "!@#$%^&*()_+-=?abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 12);
                    Console.WriteLine($"Creating credentials for user {user.UserName} with ID {user.Id}");
                    Console.WriteLine($"Password: {password}");
                    await db.AddAsync(new UserCredential(
                            userId: user.Id,
                            passwordHash: authService.PasswordHasher(password)
                        )
                    );
                }
            }

            await db.SaveChangesAsync();
        }

        public static async Task GenerateNextDayShowingsAsync(ApiDbContext db,
            CancellationToken cancellationToken = default) {
            await GenerateShowingsAsync(db, 1, appendAfterLatest: true, cancellationToken);
        }

        private static async Task EnsureInitialShowingsAsync(ApiDbContext db) {
            if (await db.Showings.AnyAsync()) {
                return;
            }

            await GenerateShowingsAsync(db, 7, appendAfterLatest: false, CancellationToken.None);
        }

        private static async Task GenerateShowingsAsync(
            ApiDbContext db,
            int daysToGenerate,
            bool appendAfterLatest,
            CancellationToken cancellationToken) {
            var movies = await db.Movies.ToListAsync(cancellationToken);
            var auditoriums = await db.Auditoriums.ToListAsync(cancellationToken);

            if (movies.Count == 0 || auditoriums.Count == 0) {
                return;
            }

            var startDate = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);

            if (appendAfterLatest && await db.Showings.AnyAsync(cancellationToken)) {
                var latestShowing = await db.Showings
                    .OrderByDescending(s => s.StartsAt)
                    .Select(s => s.StartsAt)
                    .FirstAsync(cancellationToken);

                startDate = new DateTimeOffset(latestShowing.UtcDateTime.Date, TimeSpan.Zero).AddDays(1);
            }

            var random = new Random();
            var dutchMovies = movies
                .Where(m => m.SpokenLanguageCodeIso6391 == "nl")
                .ToList();

            var kidsMovies = movies
                .Where(m => int.TryParse(m.AgeIndication, out var age) && age < 12)
                .ToList();

            var showings = new List<Showing>();

            for (int day = 0; day < daysToGenerate; day++) {
                var currentDate = startDate.AddDays(day);

                var timeSlots = new List<DateTimeOffset>();
                for (int hour = 10; hour <= 19; hour += 2) {
                    timeSlots.Add(currentDate.AddHours(hour));
                }

                var dailyCount = random.Next(6, 13);

                var selectedMovies = movies
                    .OrderBy(_ => random.Next())
                    .Take(dailyCount)
                    .ToList();

                if (selectedMovies.Count == 0) continue;

                if (dutchMovies.Any()) {
                    var m = dutchMovies[random.Next(dutchMovies.Count)];
                    if (!selectedMovies.Any(x => x.Id == m.Id)) {
                        selectedMovies[0] = m;
                    }
                }

                if (kidsMovies.Any()) {
                    var m = kidsMovies[random.Next(kidsMovies.Count)];
                    if (!selectedMovies.Any(x => x.Id == m.Id)) {
                        if (selectedMovies.Count > 1)
                            selectedMovies[1] = m;
                        else
                            selectedMovies[0] = m;
                    }
                }

                foreach (var movie in selectedMovies) {
                    var auditorium = auditoriums[random.Next(auditoriums.Count)];
                    var time = timeSlots[random.Next(timeSlots.Count)];

                    showings.Add(new Showing {
                        Movie = movie,
                        AuditoriumId = auditorium.Id,
                        Auditorium = auditorium,
                        StartsAt = time,
                        Is3D = random.Next(0, 2) == 0,
                        AuditoriumLayoutSnapshot = auditorium.RowConfigJson
                    });
                }
            }

            if (showings.Count == 0) {
                return;
            }

            await db.Showings.AddRangeAsync(showings, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
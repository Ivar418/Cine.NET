using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SharedLibrary.DTOs.Responses;
using System.Text.Json;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using SharedLibrary.DTOs.Responses.TMDB;
using Microsoft.EntityFrameworkCore;


namespace API.Infrastructure.Database
{
    using SharedLibrary.Domain.Entities;
    using System;

    public static class DbSeeder
    {
        public static async Task SeedAsync(ApiDbContext db, IMovieRepository movieRepository)
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
                var Movies = new List<TmdbMovieDetailsResponse>();

                foreach (var id in MovieIdList)
                {
                    var movie = await movieRepository.GetTmdbMovieDetailsAsync(id, "nl");
                    if (movie != null)
                    {
                        Movies.Add(movie);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch movie with id {id}");
                    }
                }

                foreach (var movie in Movies)
                {
                    await movieRepository.AddMovieAsync(movie);
                }
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
                db.Auditoriums.AddRange(
                    new Auditorium { Name = "Zaal 1" },
                    new Auditorium { Name = "Zaal 2" },
                    new Auditorium { Name = "Zaal 3" },
                    new Auditorium { Name = "Zaal 4" },
                    new Auditorium { Name = "Zaal 5" },
                    new Auditorium { Name = "Zaal 6" }
                );
            }

            await db.SaveChangesAsync();

            if (!await db.Showings.AnyAsync())
            {
                var movies = await db.Movies.ToListAsync();
                var auditoriums = await db.Auditoriums.ToListAsync();

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
                        AuditoriumLayoutSnapshot = "[]" // snapshot leeg laten
                    });
                }

                db.Showings.AddRange(showings);
                await db.SaveChangesAsync();
            }
        }
    }
}
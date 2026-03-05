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

            await db.SaveChangesAsync();
            var searchMovie = await movieRepository.GetMovieAsync(9);
            if (searchMovie.IsSuccess)
            {
                // Console.WriteLine(searchMovie.Value.Title);
                Console.WriteLine("Found movie: ");
                var options = new JsonSerializerOptions { WriteIndented = true };
                Console.WriteLine(JsonSerializer.Serialize(searchMovie, options));
            }
            else if (searchMovie.IsFailure)
            {
                Console.WriteLine("Error fetching movie: " + searchMovie.Error);
            }
        }
    }
}
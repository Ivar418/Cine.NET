using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SharedLibrary.DTOs.Responses;
using System.Text.Json;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using SharedLibrary.DTOs.Responses.TMDB;
using DotNetEnv;
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
                    var movie = await movieRepository.GetTmdbMovieDetailsAsync(id);
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
                    Console.WriteLine($"Adding movie: {movie.OriginalTitle}");
                    var firstLanguage = movie.SpokenLanguages?.FirstOrDefault();
            
                    movieEntities.Add(new Movie
                    {
                        Title = movie.OriginalTitle,
                        TmdbId = movie.Id,
                        Language = movie.OriginalLanguage,
                        PosterUrl = movie.PosterPath,
                        Runtime = movie.Runtime,
                        ImdbId = movie.ImdbId,
                        ReleaseDate = movie.ReleaseDate,
                        About = movie.Overview,
                        AgeIndication = "PG-13",
                        SpokenLanguageName = firstLanguage?.EnglishName,
                        SpokenLanguageCodeIso6391 = firstLanguage?.Iso_639_1,
                        GenresIds = movie.Genres.Select(genreDto => genreDto.Id).ToList(),
                        RowCreatedTimestampUtc = Movie.CurrentUtcTimestamp()
                    });
                }
            }

            await db.Movies.AddRangeAsync(movieEntities);
            await db.SaveChangesAsync();
        }
    }
}
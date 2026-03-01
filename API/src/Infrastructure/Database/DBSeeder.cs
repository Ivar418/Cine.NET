using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SharedLibrary.DTOs.Responses;
using System.Text.Json;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using SharedLibrary.DTOs.Responses.TMDB;
using DotNetEnv;


namespace API.Infrastructure.Database
{
    using SharedLibrary.Domain.Entities;
    using System;

    public static class DbSeeder
    {
        public static async Task SeedAsync(ApiDbContext db, IMovieRepository movieRepository)
        {
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User("Admin"),
                    new User("TestUser"),
                    new User("John Doe"),
                    new User("Jane Smith")
                );
            }

            if (!db.Movies.Any())
            {
                Console.WriteLine("Getting movie data from TMDB with id 285");
                var movie = await movieRepository.GetTmdbMovieDetailsAsync(285);
                Console.WriteLine(movie);
                db.Movies.AddRange(
                    new Movie
                    {
                        Title = movie.OriginalTitle,
                        TmdbId = movie.Id,
                        Language = movie.OriginalLanguage,
                        PosterUrl = movie.PosterPath,
                        Runtime = movie.Runtime,
                        Auditorium = "Auditorium 1",
                        ImdbId = movie.ImdbId,
                        ReleaseDate = movie.ReleaseDate,
                        About = movie.Overview,
                        AgeIndication = "PG-13",
                        SpokenLanguageName = movie.SpokenLanguages[0].EnglishName,
                        SpokenLanguageCodeIso6391 = movie.SpokenLanguages[0].Iso_639_1,
                        Genres = string.Join(", ", movie.Genres.Select(g => g.Name))
                    },
                    new Movie
                    {
                        Title = "Inception",
                        TmdbId = 27205,
                        Language = "en",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/inception.jpg",
                        Runtime = 148,
                        Auditorium = "Auditorium 1",
                        ImdbId = "tt1375666",
                        ReleaseDate = "2010-07-16",
                        About = "A skilled thief leads a team into dreams to steal secrets.",
                        AgeIndication = "PG-13",
                        SpokenLanguageName = "English",
                        SpokenLanguageCodeIso6391 = "en",
                        Genres = "Action, Science Fiction"
                    },
                    new Movie
                    {
                        Title = "The Matrix",
                        TmdbId = 603,
                        Language = "en",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/matrix.jpg",
                        Runtime = 136,
                        Auditorium = "Auditorium 2",
                        ImdbId = "tt0133093",
                        ReleaseDate = "1999-03-31",
                        About = "A hacker discovers reality is a simulation.",
                        AgeIndication = "R",
                        SpokenLanguageName = "English",
                        SpokenLanguageCodeIso6391 = "en",
                        Genres = "Action, Science Fiction"
                    },
                    new Movie
                    {
                        Title = "Interstellar",
                        TmdbId = 157336,
                        Language = "en",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/interstellar.jpg",
                        Runtime = 169,
                        Auditorium = "Auditorium 3",
                        ImdbId = "tt0816692",
                        ReleaseDate = "2014-11-07",
                        About = "Explorers travel through a wormhole in space.",
                        AgeIndication = "PG-13",
                        SpokenLanguageName = "English",
                        SpokenLanguageCodeIso6391 = "en",
                        Genres = "Adventure, Drama, Science Fiction"
                    }
                );
            }

            db.SaveChanges();
        }
    }
}
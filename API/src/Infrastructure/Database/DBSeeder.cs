namespace API.Infrastructure.Database;

using SharedLibrary.Domain.Entities;

public static class DbSeeder
{
    public static void Seed(ApiDbContext context)
    {
        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User("Admin"),
                new User("TestUser"),
                new User("John Doe"),
                new User("Jane Smith")
            );
        }

        if (!context.Movies.Any())
        {
            context.Movies.AddRange(
                new Movie
                {
                    Title = "Inception",
                    TmdbId = "27205",
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
                    TmdbId = "603",
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
                    TmdbId = "157336",
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

        context.SaveChanges();
    }
}
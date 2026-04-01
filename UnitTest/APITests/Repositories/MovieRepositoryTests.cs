using API.Repositories.Implementations;
using SharedLibrary.Domain.Entities;
using Xunit;
using UnitTest.Helpers;
using System.Linq;

namespace UnitTest.APITests.Repositories;

public class MovieRepositoryTests
{
    private static Movie BuildMovie(int id = 1, int tmdbId = 100)
    {
        return new Movie
        {
            Id = id,
            Title = "Test Movie",
            TmdbId = tmdbId,
            InformationLanguage = "en",
            RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+00:00"
        };
    }

    // -------------------------------------------------------
    // GetMovieAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetMovieAsync_WhenExists_ReturnsSuccess()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetMovieAsync_WhenExists_ReturnsSuccess));
        var movie = BuildMovie();

        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        var repo = new MovieRepository(db);

        var result = await repo.GetMovieAsync(movie.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(movie.Id, result.Value!.Id);
    }

    [Fact]
    public async Task GetMovieAsync_WhenNotExists_ReturnsFailure()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetMovieAsync_WhenNotExists_ReturnsFailure));
        var repo = new MovieRepository(db);

        var result = await repo.GetMovieAsync(999);

        Assert.True(result.IsFailure);
        Assert.Equal("Movie not found", result.Error);
    }

    // -------------------------------------------------------
    // GetMoviesByTmdbIdAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetMoviesByTmdbIdAsync_WhenFound_ReturnsMovies()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetMoviesByTmdbIdAsync_WhenFound_ReturnsMovies));

        db.Movies.AddRange(
            BuildMovie(1, 10),
            BuildMovie(2, 10)
        );
        await db.SaveChangesAsync();

        var repo = new MovieRepository(db);

        var result = await repo.GetMoviesByTmdbIdAsync(10);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetMoviesByTmdbIdAsync_WhenNotFound_ReturnsFailure()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetMoviesByTmdbIdAsync_WhenNotFound_ReturnsFailure));
        var repo = new MovieRepository(db);

        var result = await repo.GetMoviesByTmdbIdAsync(999);

        Assert.True(result.IsFailure);
    }

    // -------------------------------------------------------
    // GetMoviesAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetMoviesAsync_All_ReturnsAllMovies()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetMoviesAsync_All_ReturnsAllMovies));

        db.Movies.AddRange(
            BuildMovie(1),
            BuildMovie(2)
        );
        await db.SaveChangesAsync();

        var repo = new MovieRepository(db);

        var result = await repo.GetMoviesAsync("all");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task GetMoviesAsync_Filtered_ReturnsFilteredMovies()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetMoviesAsync_Filtered_ReturnsFilteredMovies));

        var enMovie = BuildMovie(1);
        enMovie.InformationLanguage = "en";

        var nlMovie = BuildMovie(2);
        nlMovie.InformationLanguage = "nl";

        db.Movies.AddRange(
            enMovie,
            nlMovie
        );
        await db.SaveChangesAsync();

        var repo = new MovieRepository(db);

        var result = await repo.GetMoviesAsync("nl");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    // -------------------------------------------------------
    // DeleteMovieByTmdbIdAsync
    // -------------------------------------------------------

    [Fact]
    public async Task DeleteMovieByTmdbIdAsync_WhenExists_DeletesMovie()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(DeleteMovieByTmdbIdAsync_WhenExists_DeletesMovie));

        var movie = BuildMovie(1, 55);
        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        var repo = new MovieRepository(db);

        var result = await repo.DeleteMovieByTmdbIdAsync(55);

        Assert.True(result.IsSuccess);
        Assert.Empty(db.Movies);
    }

    [Fact]
    public async Task DeleteMovieByTmdbIdAsync_WhenNotExists_ReturnsFailure()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(DeleteMovieByTmdbIdAsync_WhenNotExists_ReturnsFailure));
        var repo = new MovieRepository(db);

        var result = await repo.DeleteMovieByTmdbIdAsync(123);

        Assert.True(result.IsFailure);
    }

    // -------------------------------------------------------
    // Genres
    // -------------------------------------------------------

    [Fact]
    public async Task SaveGenres_SavesGenres()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(SaveGenres_SavesGenres));
        var repo = new MovieRepository(db);

        var genres = new List<Genre>
        {
            new Genre { Name = "Action", TmdbId = 1, Language = "en" }
        };

        var result = await repo.SaveGenres(genres);

        Assert.True(result.IsSuccess);
        Assert.Single(db.Genres);
    }

    [Fact]
    public async Task GetGenreByTmdbGenreId_WhenExists_ReturnsGenre()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetGenreByTmdbGenreId_WhenExists_ReturnsGenre));

        db.Genres.Add(new Genre { TmdbId = 1, Name = "Action", Language = "en" });
        await db.SaveChangesAsync();

        var repo = new MovieRepository(db);

        var result = await repo.GetGenreByTmdbGenreId(1, "en");

        Assert.True(result.IsSuccess);
        Assert.Equal("Action", result.Value!.Name);
    }

    [Fact]
    public async Task GetGenreByTmdbGenreId_WhenNotExists_ReturnsFailure()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetGenreByTmdbGenreId_WhenNotExists_ReturnsFailure));
        var repo = new MovieRepository(db);

        var result = await repo.GetGenreByTmdbGenreId(999, "en");

        Assert.True(result.IsFailure);
    }
}
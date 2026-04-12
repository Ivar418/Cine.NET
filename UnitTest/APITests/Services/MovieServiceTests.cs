using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using Moq;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.TMDB.Genre;
using Xunit;

namespace UnitTest.APITests.Services;

public class MovieServiceTests
{
    private readonly Mock<IMovieRepository> _repoMock;
    private readonly MovieService _sut;

    public MovieServiceTests()
    {
        _repoMock = new Mock<IMovieRepository>();
        _sut = new MovieService(_repoMock.Object);
    }

    private static Movie BuildMovie(int id = 1) => new Movie
    {
        Id = id,
        Title = "Test",
        TmdbId = 123,
        RowCreatedTimestampUtc = "2026-01-01T00:00:00+00:00"
    };
    
    // -------------------------------------------------------
    // FetchGenreByLanguage
    // -------------------------------------------------------

    [Fact]
    public async Task FetchGenreByLanguage_WhenExists_ReturnsGenre()
    {
        var genre = new Genre { TmdbId = 1, Name = "Action", Language = "en" };

        _repoMock
            .Setup(r => r.GetAllGenresOnDb())
            .ReturnsAsync(ResultOf<IEnumerable<Genre>>.Success(new List<Genre> { genre }));

        var result = await _sut.FetchGenreByLanguage(1, "en");

        Assert.True(result.IsSuccess);
        Assert.Equal("Action", result.Value.Name);
    }

    [Fact]
    public async Task FetchGenreByLanguage_WhenNotExists_FetchesFromRepo()
    {
        _repoMock
            .Setup(r => r.GetAllGenresOnDb())
            .ReturnsAsync(ResultOf<IEnumerable<Genre>>.Success(new List<Genre>()));

        var fetched = new Genre { TmdbId = 1, Name = "Action", Language = "en" };

        _repoMock
            .Setup(r => r.SaveGenreByTmdbGenreId("en", 1))
            .ReturnsAsync(ResultOf<IEnumerable<Genre>>.Success(new List<Genre> { fetched }));

        var result = await _sut.FetchGenreByLanguage(1, "en");

        Assert.True(result.IsSuccess);
        Assert.Equal("Action", result.Value.Name);
    }

    // -------------------------------------------------------
    // FetchAllGenresForAllSpecifiedLanguagesAndSaveToDb
    // -------------------------------------------------------

    [Fact]
    public async Task FetchAllGenres_Success_ReturnsGenres()
    {
        var tmdbGenres = new List<GenreItem>
        {
            new GenreItem { Id = 1, Name = "Action" }
        };

        var tmdbResult = new GenreResultList
        {
            Genres = tmdbGenres
        };

        _repoMock
            .Setup(r => r.GetAllGenresFromTmdb(It.IsAny<string>()))
            .ReturnsAsync(ResultOf<GenreResultList>.Success(tmdbResult));

        _repoMock
            .Setup(r => r.SaveGenres(It.IsAny<IEnumerable<Genre>>()))
            .ReturnsAsync(ResultOf<IEnumerable<Genre>>.Success(new List<Genre>()));

        _repoMock
            .Setup(r => r.GetAllGenresOnDb())
            .ReturnsAsync(ResultOf<IEnumerable<Genre>>.Success(new List<Genre>()));

        var result = await _sut.FetchAllGenresForAllSpecifiedLanguagesAndSaveToDb();

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task FetchAllGenres_WhenDbFails_ReturnsFailure()
    {
        _repoMock
            .Setup(r => r.GetAllGenresFromTmdb(It.IsAny<string>()))
            .ReturnsAsync(ResultOf<GenreResultList>.Success(new GenreResultList
            {
                Genres = new List<GenreItem>()
            }));

        _repoMock
            .Setup(r => r.SaveGenres(It.IsAny<IEnumerable<Genre>>()))
            .ReturnsAsync(ResultOf<IEnumerable<Genre>>.Success(new List<Genre>()));

        _repoMock
            .Setup(r => r.GetAllGenresOnDb())
            .ReturnsAsync(ResultOf<IEnumerable<Genre>>.Failure("db error"));

        var result = await _sut.FetchAllGenresForAllSpecifiedLanguagesAndSaveToDb();

        Assert.True(result.IsFailure);
    }
}
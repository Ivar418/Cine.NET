using API.Controllers;
using API.Domain.Common;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedLibrary.Domain.Entities;
using Xunit;

namespace UnitTest.APITests.Controllers;

public class MoviesControllerTests
{
    private readonly Mock<IMovieService> _movieServiceMock;
    private readonly MoviesController _sut;
    public MoviesControllerTests()
    {
        _movieServiceMock = new Mock<IMovieService>();
        _sut = new MoviesController(_movieServiceMock.Object);
    }
    
    private static Movie BuildMovie(int id = 1, string title = "Inception") => new Movie
    {
        Id = id,
        Title = title,
        TmdbId = 27205,
        RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+00:00"
    };

    // -------------------------------------------------------
    // GET /api/movies
    // -------------------------------------------------------

    [Fact]
    public async Task GetAll_WhenMoviesExist_ReturnsOkWithMovies()
    {
        var movies = new List<Movie> { BuildMovie(1, "Inception"), BuildMovie(2, "Interstellar") };
        _movieServiceMock
            .Setup(s => s.GetMoviesAsync("all"))
            .ReturnsAsync(ResultOf<ICollection<Movie>>.Success(movies));
        
        var result = await _sut.GetAll("all");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(movies, ok.Value);
    }

    [Fact]
    public async Task GetAll_WhenServiceFails_Returns500()
    {
        _movieServiceMock
            .Setup(s => s.GetMoviesAsync("all"))
            .ReturnsAsync(ResultOf<ICollection<Movie>>.Failure("Database error"));
        
        var result = await _sut.GetAll("all");
        
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // -------------------------------------------------------
    // GET /api/movies/{id}
    // -------------------------------------------------------

    [Fact]
    public async Task GetMovieById_WhenMovieExists_ReturnsOkWithMovie()
    {
        var movie = BuildMovie(1, "Inception");
        _movieServiceMock
            .Setup(s => s.GetMovieAsync(1))
            .ReturnsAsync(ResultOf<Movie>.Success(movie));
        
        var result = await _sut.GetMovieById(1);
        
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(movie, ok.Value);
    }

    [Fact]
    public async Task GetMovieById_WhenMovieDoesNotExist_ReturnsNotFound()
    {
        _movieServiceMock
            .Setup(s => s.GetMovieAsync(99))
            .ReturnsAsync(ResultOf<Movie>.Failure("Movie not found"));
        
        var result = await _sut.GetMovieById(99);
        
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetMovieById_WhenServiceFails_Returns500()
    {
        _movieServiceMock
            .Setup(s => s.GetMovieAsync(1))
            .ReturnsAsync(ResultOf<Movie>.Failure("Database error"));
        
        var result = await _sut.GetMovieById(1);
        
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // -------------------------------------------------------
    // DELETE /api/movies/{tmdbId}
    // -------------------------------------------------------

    [Fact]
    public async Task DeleteByTmdbId_WhenMovieExists_ReturnsOk()
    {
        var movie = BuildMovie(1, "Inception");
        _movieServiceMock
            .Setup(s => s.DeleteMovieByTmdbIdAsync(27205))
            .ReturnsAsync(ResultOf<Movie>.Success(movie));

        var result = await _sut.DeleteByTmdbId(27205);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteByTmdbId_WhenMovieDoesNotExist_ReturnsNotFound()
    {
        _movieServiceMock
            .Setup(s => s.DeleteMovieByTmdbIdAsync(99999))
            .ReturnsAsync(ResultOf<Movie>.Failure("Movie not found"));
        
        var result = await _sut.DeleteByTmdbId(99999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // -------------------------------------------------------
    // POST /api/movies
    // -------------------------------------------------------

    [Fact]
    public async Task AddMovieByTmdbId_InvalidTmdbId_ReturnsBadRequest()
    {
        var result = await _sut.AddMovieByTmdbId(-1);

        Assert.IsType<BadRequestObjectResult>(result);
        
        _movieServiceMock.Verify(
            s => s.AddMovieAsyncForEachSpecifiedLanguage(It.IsAny<int>(), It.IsAny<IEnumerable<string>>()),
            Times.Never);
    }

    [Fact]
    public async Task AddMovieByTmdbId_WhenMovieAdded_ReturnsCreated()
    {
        var movies = new List<Movie> { BuildMovie(1, "Inception") };
        _movieServiceMock
            .Setup(s => s.AddMovieAsyncForEachSpecifiedLanguage(27205, null))
            .ReturnsAsync(ResultOf<IEnumerable<Movie>>.Success(movies));
        
        var result = await _sut.AddMovieByTmdbId(27205);
        
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task AddMovieByTmdbId_WhenMovieAlreadyExists_ReturnsConflict()
    {
        _movieServiceMock
            .Setup(s => s.AddMovieAsyncForEachSpecifiedLanguage(27205, null))
            .ReturnsAsync(ResultOf<IEnumerable<Movie>>.Success(new List<Movie>()));
        
        var result = await _sut.AddMovieByTmdbId(27205);
        
        Assert.IsType<ConflictObjectResult>(result);
    }
}
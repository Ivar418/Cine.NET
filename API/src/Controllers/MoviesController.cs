using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Domain.Entities;

namespace API.Controllers;

/// <summary>
/// Controller responsible for managing movie-related operations and exposing endpoints for movie data interactions.
/// </summary>
/// <remarks>
/// Provides functionality to perform operations such as retrieving all movies, deleting movies by their TMDB ID,
/// fetching movie details by internal ID, searching for movies using an external service, and adding movies by their TMDB ID.
/// Interfaces with the IMovieService to handle required business logic and data operations.
/// </remarks>
[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    /// <summary>
    /// Provides access to movie-related operations and data retrieval logic.
    /// This field is initialized through dependency injection and is used
    /// to interact with an implementation of the <see cref="IMovieService"/> interface.
    /// Responsible for handling tasks such as retrieving, searching, creating,
    /// updating, and deleting movie information.
    /// </summary>
    private readonly IMovieService _movieService;

    /// <summary>
    /// A controller responsible for managing movie-related operations.
    /// Provides endpoints for retrieving, searching, and managing movie data,
    /// including integration with external APIs.
    /// </summary>
    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }


    /// <summary>
    /// Retrieves a list of all movies from the repository.
    /// </summary>
    /// <param name="language">Optional language parameter to specify the language of the movies. Defaults to "nl".</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a list of movies on success,
    /// or an error message with appropriate HTTP status on failure.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string language = "all")
    {
        try
        {
            var movies = await _movieService.GetMoviesAsync(language);
            return movies switch
            {
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(movies.Value),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    /// <summary>
    /// Deletes a movie identified by its TmdbId from the database.
    /// </summary>
    /// <param name="tmdbId">The unique identifier (TmdbId) of the movie to be deleted.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the outcome of the operation:
    /// - Returns <c>200 OK</c> if the movie was successfully deleted.
    /// - Returns <c>404 Not Found</c> if a movie with the specified TmdbId does not exist.
    /// - Returns <c>500 Internal Server Error</c> in case of unexpected errors.
    /// </returns>
    [HttpDelete]
    [Route("{tmdbId:int}")]
    public async Task<IActionResult> DeleteByTmdbId(int tmdbId)
    {
        try
        {
            var result = await _movieService.DeleteMovieByTmdbIdAsync(tmdbId);
            return result switch
            {
                { IsFailure: true, Error: "Movie not found" } => NotFound($"Movie with TmdbId {tmdbId} not found"),
                { IsSuccess: true } => Ok($"Movie with tmdbId {tmdbId} and title {result.Value.Title} deleted"),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred" });
        }
    }


    /// <summary>
    /// Retrieves a movie by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the movie to retrieve.</param>
    /// <returns>
    /// An IActionResult containing the movie details if found,
    /// a 404 Not Found response if the movie is not found,
    /// or a 500 Internal Server Error response if an unexpected error occurs.
    /// </returns>
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetMovieById(int id)
    {
        try
        {
            var movie = await _movieService.GetMovieAsync(id);
            return movie switch
            {
                { IsFailure: true, Error: "Movie not found" } => NotFound(new { error = "Movie not found" }),
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(movie.Value),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    /// <summary>
    /// Searches for movies in TMDB (The Movie Database) based on the provided query parameters
    /// and returns the matching search results.
    /// </summary>
    /// <param name="query">The search query string used to look for movies in TMDB.</param>
    /// <param name="primary_release_year">Optional. The primary release year to filter the search results.</param>
    /// <param name="page">Optional. The page number of the search results to retrieve. Defaults to 1.</param>
    /// <param name="include_adult">Optional. A value indicating whether to include adult content in the search results. Defaults to false.</param>
    /// <param name="language">Optional. The language in which the search results are returned. Defaults to "nl".</param>
    /// <returns>
    /// An IActionResult containing the search results from TMDB or an error response in case of a failure.
    /// </returns>
    [HttpGet]
    [Route("tmdb/search")]
    public async Task<IActionResult> SearchTmdb(
        [FromQuery] string query,
        [FromQuery] string? primary_release_year,
        [FromQuery] int? page,
        [FromQuery] bool include_adult = false,
        [FromQuery] string language = "nl")
    {
        try
        {
            var result = await _movieService.GetMovieTmdbSearchResultsAsync(query, primary_release_year, page,
                include_adult, language);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    /// <summary>
    /// Adds a movie to the database using its TMDB ID and retrieves its details from TMDB.
    /// The movie can be stored in one or more specified languages.
    /// </summary>
    /// <param name="tmdbId">The TMDB ID of the movie to be added. Must be a positive integer.</param>
    /// <param name="language">A comma-separated list of languages for which the movie information
    /// should be retrieved and stored. If null or empty, the default language(s) will be used.</param>
    /// <returns>
    /// Returns an HTTP response indicating the result of the operation:
    /// - 201 Created if the movie was successfully added.
    /// - 409 Conflict if no new movies were added because they already exist.
    /// - 404 Not Found if the movie could not be located locally or on TMDB.
    /// - 400 Bad Request if the supplied TMDB ID is invalid.
    /// - 500 Internal Server Error for any unexpected errors.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> AddMovieByTmdbId(
        [FromQuery] int tmdbId,
        [FromQuery] string? language = null)
    {
        string[]? languages = string.IsNullOrWhiteSpace(language)
            ? null
            : language.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .ToArray();

        if (tmdbId <= 0)
            return BadRequest(new { error = "Invalid TMDB id" });

        try
        {
            var result = await _movieService.AddMovieAsyncForEachSpecifiedLanguage(tmdbId, languages);

            if (result.IsFailure)
                return StatusCode(500, new { error = "An error occurred" });

            var movies = result.Value?.ToList();

            if (movies == null || !movies.Any())
                return Conflict(new { message = "No new movies were added" });

            return CreatedAtAction(
                nameof(GetMovieById),
                new { id = movies.First().Id },
                movies);  // ✅ pass list directly — serializer handles it
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    /// <summary>
    /// Retrieves a movie genre based on the specified TMDB genre ID and language.
    /// </summary>
    /// <param name="genreId">The unique TMDB genre ID for the genre to retrieve.</param>
    /// <param name="language">The language in which the genre name should be returned. Defaults to "nl".</param>
    /// <returns>An <see cref="IActionResult"/> containing the genre details if found, or an appropriate error message if not.</returns>
    [Route("genres/{genreId:int}")]
    [HttpGet]
    public async Task<IActionResult> GetGenreByTmdbGenreId(int genreId, [FromQuery] string language = "nl")
    {
        try
        {
            var genre = await _movieService.FetchGenreByLanguage(genreId, language);
            return genre switch
            {
                { IsFailure: true, Error: "Genre not found" } => NotFound(new { error = "Genre not found" }),
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(genre.Value),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred" });
        }
    }
}
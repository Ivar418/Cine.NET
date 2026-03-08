using API.Domain.Common;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Domain.Entities;

namespace API.Controllers
{
    /// <summary>
    /// Controller for managing movie-related operations and handling movie requests.
    /// </summary>
    /// <remarks>
    /// The MoviesController provides endpoints to retrieve all movies, fetch a specific movie by its ID,
    /// search for movies using an external API, and add a movie to the system based on external data.
    /// All actions rely on the injected IMovieRepository for data operations.
    /// </remarks>
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        /// <summary>
        /// Represents the repository responsible for providing data access functionalities
        /// related to movies. The repository abstracts interactions with the data sources,
        /// such as databases or external APIs, to perform operations including retrieval,
        /// search, creation, updating, and deletion of movie records.
        /// </summary>
        private readonly IMovieRepository _movieRepository;

        /// <summary>
        /// A controller for managing movie-related operations, providing endpoints to retrieve,
        /// search, and manage movie data.
        /// </summary>
        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }


        /// Retrieves a list of all movies from the repository.
        /// The method attempts to fetch all movies using the injected repository.
        /// If the operation fails, it will return an appropriate HTTP status code
        /// indicating the error (e.g., 500 Internal Server Error). If successful,
        /// it returns the list of movies.
        /// <return>Returns an IActionResult containing a list of movies on success, or an error message on failure.</return>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var movies = await _movieRepository.GetMoviesAsync();
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

        [HttpDelete]
        [Route("{tmdbId:int}")]

        public async Task<IActionResult> DeleteByTmdbId(int tmdbId)
        {
            var result = await _movieRepository.DeleteMovieAsync(tmdbId);
            return result switch
            {
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok($"Movie with tmdbId {tmdbId} and title {result.Value.Title} deleted"),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }


        /// Retrieves a movie by its unique identifier.
        /// <param name="id">The unique identifier of the movie to retrieve.</param>
        /// <returns>An IActionResult containing the movie details if found, a 404 Not Found response if the movie is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMovieAsync(id);
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
        /// Searches for movies in TMDB (The Movie Database) based on the provided query string and returns the matching Results.
        /// </summary>
        /// <param name="query">The search query string used to look for movies in TMDB.</param>
        /// <param name="primary_release_year">The primary release year to filter the search Results. Optional.</param>
        /// <param name="page">The page number of the search Results to retrieve. Defaults to 1.</param>
        /// <param name="include_adult">A boolean value indicating whether to include adult content in the search Results. Defaults to false.</param>
        /// <param name="language">The language in which the search Results are returned. Defaults to "nl".</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the search Results from TMDB or an error response if an issue occurs during the search.
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
                var result = await _movieRepository.GetMovieTmdbSearchResultsAsync(query, primary_release_year, page,
                    include_adult, language);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Adds a movie to the database based on its TMDB ID.
        /// </summary>
        /// <param name="tmdbId">The TMDB ID of the movie to be added. Must be a positive integer.</param>
        /// <returns>
        /// Returns a status indicating the result of the operation:
        /// - 201 Created if the movie was successfully added.
        /// - 409 Conflict if the movie already exists in the database.
        /// - 404 Not Found if the movie is not found locally or on TMDB.
        /// - 400 Bad Request if the TMDB ID is invalid.
        /// - 500 Internal Server Error in case of any unexpected errors.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddMovie(
            [FromQuery] int tmdbId,
            [FromQuery] string language = "nl")

        {
            if (tmdbId <= 0)
                return BadRequest(new { error = "Invalid TMDB id" });

            try
            {
                // Repository should: fetch TMDB details, map to Movie, save, return Result<Movie>
                var result = await _movieRepository.AddMovieFromTmdbAsync(tmdbId, language);

                return result switch
                {
                    { IsFailure: true, Error: "Movie already exists" } => Conflict(new
                        { error = "Movie already exists" }),
                    { IsFailure: true, Error: "Movie not found" } =>
                        NotFound(new { error = "Movie not found" }),
                    { IsFailure: true, Error: "Movie not found on TMDB" } => NotFound(new
                        { error = "Movie not found on TMDB" }),
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => CreatedAtAction(nameof(GetMovieById), new { id = result.Value.Id },
                        result.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}
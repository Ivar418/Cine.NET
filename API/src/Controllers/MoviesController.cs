using API.Domain.Common;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Domain.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }


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

        [HttpGet]
        [Route("tmdb/search")]
        public async Task<IActionResult> SearchTmdb([FromQuery] string query)
        {
            try
            {
                var result = await _movieRepository.GetMovieTmdbSearchResultsAsync(query);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromQuery] int tmdbId)
        {
            if (tmdbId <= 0)
                return BadRequest(new { error = "Invalid TMDB id" });

            try
            {
                // Repository should: fetch TMDB details, map to Movie, save, return Result<Movie>
                var result = await _movieRepository.AddMovieFromTmdbAsync(tmdbId);

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
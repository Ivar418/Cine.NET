using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
    }
}
using API.Services;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/showings")]
    public class ShowingController : ControllerBase
    {
        /// <summary>
        /// Represents the repository responsible for providing data access functionalities
        /// related to Showings. The repository abstracts interactions with the data sources,
        /// such as databases or external APIs, to perform operations including retrieval,
        /// search, creation, updating, and deletion of Showing records.
        /// </summary>
        private readonly IShowingRepository _ShowingRepository;

        private readonly IShowingService _showingService;

        /// <summary>
        /// A controller for managing Showing-related operations, providing endpoints to retrieve,
        /// and manage Showing data.
        /// </summary>
        public ShowingController(IShowingRepository showingRepository, IShowingService showingService)
        {
            _ShowingRepository = showingRepository;
            _showingService = showingService;
        }

        /// <summary>
        /// Retrieves all showings.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of showings when successful,
        /// or an error response when retrieval fails.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var Showings = await _ShowingRepository.GetShowingsAsync();
                return Showings switch
                {
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(Showings.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Retrieves all showings including pricing information.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing showings with prices,
        /// or a <c>500 Internal Server Error</c> response when retrieval fails.
        /// </returns>
        [HttpGet("with-prices")]
        public async Task<IActionResult> GetShowingsWithPrices()
        {
            var result = await _showingService.GetShowingsAsync();

            return result switch
            {
                { IsFailure: true } => StatusCode(500, "An error occurred"),
                { IsSuccess: true } => Ok(result.Value),
                _ => StatusCode(500)
            };
        }

        /// <summary>
        /// Retrieves a single showing including pricing information.
        /// </summary>
        /// <param name="id">The showing identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the showing with prices,
        /// <c>404 Not Found</c> when the showing does not exist,
        /// or <c>500 Internal Server Error</c> when an unexpected error occurs.
        /// </returns>
        [HttpGet("{id}/prices")]
        public async Task<IActionResult> GetShowingWithPrices(int id)
        {
            var result = await _showingService.GetShowingAsync(id);

            return result switch
            {
                { IsFailure: true, Error: "NotFound" } => NotFound("Not found"),
                { IsFailure: true } => StatusCode(500, "An error occurred"),
                { IsSuccess: true } => Ok(result.Value),
                _ => StatusCode(500)
            };
        }

        /// <summary>
        /// Retrieves the current runtime state of a showing.
        /// </summary>
        /// <param name="ShowingId">The showing identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the showing state,
        /// or a <c>500 Internal Server Error</c> response when an unexpected error occurs.
        /// </returns>
        [HttpGet]
        [Route("{ShowingId:int}/state")]
        public async Task<IActionResult> GetShowingStateById(int ShowingId)
        {
            try
            {
                var result = await _showingService.GetShowingStateAsync(ShowingId);
                return result switch
                {
                    { IsFailure: true } => StatusCode(500, new { error = result.Error }),
                    { IsSuccess: true } => Ok(result.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server error");
                Console.WriteLine(ex);
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Deletes a showing by its internal identifier.
        /// </summary>
        /// <param name="ShowingId">The unique showing identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// Returns <c>200 OK</c> on success, <c>404 Not Found</c> when no showing exists for the ID,
        /// or <c>500 Internal Server Error</c> when an unexpected error occurs.
        /// </returns>
        [HttpDelete]
        [Route("{ShowingId:int}")]
        public async Task<IActionResult> DeleteById(int ShowingId)
        {
            try
            {
                var result = await _ShowingRepository.DeleteShowingByIdAsync(ShowingId);
                return result switch
                {
                    { IsFailure: true, Error: "Showing not found" } => NotFound(
                        $"Showing with TmdbId {ShowingId} not found"),
                    { IsSuccess: true } => Ok($"Showing with tmdbId {ShowingId} and title {result.Value} deleted"),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Retrieves a showing by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the Showing to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the showing details when found,
        /// or an error response when the showing is not found or retrieval fails.
        /// </returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetShowingById(int id)
        {
            try
            {
                var Showing = await _ShowingRepository.GetShowingAsync(id);
                return Showing switch
                {
                    { IsFailure: true, Error: "Showing not found" } => NotFound(new { error = "Showing not found" }),
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(Showing.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Creates a new showing.
        /// </summary>
        /// <param name="movieId">The movie identifier to schedule.</param>
        /// <param name="auditoriumId">The auditorium identifier where the movie will be shown.</param>
        /// <param name="startsAt">The start date and time of the showing.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created showing,
        /// or an error response when creation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddShowingById(
            [FromQuery] int movieId,
            [FromQuery] int auditoriumId,
            [FromQuery] DateTimeOffset startsAt)
        {
            try
            {
                var result =
                    await _ShowingRepository.AddShowingAsync(new CreateShowingRequest(movieId, auditoriumId, startsAt));
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Retrieves display-oriented details for a single showing.
        /// </summary>
        /// <param name="id">The showing identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing showing display details,
        /// <c>404 Not Found</c> when no showing exists for the identifier,
        /// or <c>500 Internal Server Error</c> when an unexpected error occurs.
        /// </returns>
        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetShowingDisplayById(int id)
        {
            try
            {
                var result = await _ShowingRepository.GetShowingDisplayByIdAsync(id);
                return result switch
                {
                    { IsFailure: true, Error: "Showing not found" } => NotFound(new { error = "Showing not found" }),
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(result.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Retrieves all upcoming showings for a specific movie, ordered by start time ascending.
        /// A showing is considered upcoming if it starts no more than 15 minutes before the current time,
        /// allowing users to still book tickets shortly after a showing has started.
        /// </summary>
        /// <param name="movieId">The internal ID of the movie to retrieve upcoming showings for.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of showing responses on success.
        /// Returns an empty list if no upcoming showings are scheduled — this is not treated as a 404.
        /// Returns <c>500 Internal Server Error</c> if an unexpected error occurs.
        /// </returns>
        [HttpGet("movie/{movieId:int}/upcoming")]
        public async Task<IActionResult> GetUpcomingShowingsByMovieId(int movieId)
        {
            var result = await _showingService.GetUpcomingShowingsByMovieIdAsync(movieId);

            return result switch
            {
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(result.Value),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }

        /// <summary>
        /// Retrieves display-oriented details for showings, optionally filtered by date.
        /// </summary>
        /// <param name="date">Optional date filter. When omitted, display data for all relevant dates is returned.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing showing display details,
        /// or a <c>500 Internal Server Error</c> response when retrieval fails.
        /// </returns>
        [HttpGet("details")]
        public async Task<IActionResult> GetShowingDisplay([FromQuery] DateOnly? date = null)
        {
            var result = await _showingService.GetShowingDisplayAsync(date);

            return result switch
            {
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(result.Value),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }

        /// <summary>
        /// Retrieves a random showing with a specified minimum number of available seats.
        /// </summary>
        /// <param name="seatsNeededAmount">The minimum number of  seats required to be available for the showing.</param>
        /// <return>Returns an IActionResult containing the details of the random showing if successful, or an error message in case of failure.</return>
        [HttpGet("randomShowingWithSeatsAvailable")]
        public async Task<IActionResult> GetRandomShowingWithAmountOfSeatsAvailable([FromQuery] int seatsNeededAmount)
        {
            var result = await _showingService.GetRandomShowingWithAmountOfSeatsAvailableAsync(seatsNeededAmount);
            return result switch
            {
                {IsFailure: true, Error: "No Showings with enough seats available"} => NotFound(new { error = "No showings with enough seats available" }),
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(result.Value),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }
    }
}
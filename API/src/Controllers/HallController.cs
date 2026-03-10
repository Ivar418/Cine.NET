
using API.src.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Controllers
{
    [ApiController]
    [Route("api/halls")]
    public class HallController : ControllerBase
    {
        /// <summary>
        /// Represents the repository responsible for providing data access functionalities
        /// related to halls. The repository abstracts interactions with the data sources,
        /// such as databases or external APIs, to perform operations including retrieval,
        /// search, creation, updating, and deletion of hall records.
        /// </summary>
        private readonly IHallRepository _hallRepository;

        /// <summary>
        /// A controller for managing hall-related operations, providing endpoints to retrieve,
        /// and manage hall data.
        /// </summary>
        public HallController(IHallRepository hallRepository)
        {
            _hallRepository = hallRepository;
        }


        /// Retrieves a list of all halls from the repository.
        /// The method attempts to fetch all halls using the injected repository.
        /// If the operation fails, it will return an appropriate HTTP status code
        /// indicating the error (e.g., 500 Internal Server Error). If successful,
        /// it returns the list of halls.
        /// <return>Returns an IActionResult containing a list of halls on success, or an error message on failure.</return>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var halls = await _hallRepository.GetHallsAsync();
                return halls switch
                {
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(halls.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Deletes a hall from the system based on its TmdbId.
        /// </summary>
        /// <param name="tmdbId">The unique TmdbId of the hall to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation. Returns:
        /// - <c>200 OK</c> if the hall was successfully deleted.
        /// - <c>404 Not Found</c> if the hall with the specified TmdbId was not found.
        /// - <c>500 Internal Server Error</c> if an unexpected error occurs.
        /// </returns>
        [HttpDelete]
        [Route("{hallId:int}")]
        public async Task<IActionResult> DeleteById(int hallId)
        {
            try
            {
                var result = await _hallRepository.DeleteHallByIdAsync(hallId);
                return result switch
                {
                    { IsFailure: true, Error: "hall not found" } => NotFound($"hall with TmdbId {hallId} not found"),
                    { IsSuccess: true } => Ok($"hall with tmdbId {hallId} and title {result.Value} deleted"),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }


        /// Retrieves a hall by its unique identifier.
        /// <param name="id">The unique identifier of the hall to retrieve.</param>
        /// <returns>An IActionResult containing the hall details if found, a 404 Not Found response if the hall is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetHallById(int id)
        {
            try
            {
                var hall = await _hallRepository.GetHallAsync(id);
                return hall switch
                {
                    { IsFailure: true, Error: "Hall not found" } => NotFound(new { error = "Hall not found" }),
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(hall.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Adds a hall to the database.
        /// </summary>
        /// <param name="name">The hall name to be added. Must be a positive integer. that does not exist</param>
        /// <param name="rows">The list of row configurations for the hall. Each row configuration should specify the number of seats and wheelchair spaces.</param>
        /// <returns>
        /// Returns a status indicating the result of the operation:
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddHallById( 
            [FromQuery] string name,
            [FromQuery] List<RowConfig> rows)
        { 
            try
            {
                var result = await _hallRepository.AddHallAsync(new CreateHallRequest(name, rows));
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}

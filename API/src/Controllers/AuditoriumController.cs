using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auditoriums")]
    public class AuditoriumController : ControllerBase
    {
        /// <summary>
        /// Represents the repository responsible for providing data access functionalities
        /// related to Auditoriums. The repository abstracts interactions with the data sources,
        /// such as databases or external APIs, to perform operations including retrieval,
        /// search, creation, updating, and deletion of Auditorium records.
        /// </summary>
        private readonly IAuditoriumService _AuditoriumService;

        /// <summary>
        /// A controller for managing Auditorium-related operations, providing endpoints to retrieve,
        /// and manage Auditorium data.
        /// </summary>
        public AuditoriumController(IAuditoriumService auditoriumService)
        {
            _AuditoriumService = auditoriumService;
        }

        /// <summary>
        /// Retrieves all auditoriums.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing all auditoriums when successful,
        /// or an error response when retrieval fails.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var Auditoriums = await _AuditoriumService.GetAuditoriumsAsync();
                return Auditoriums switch
                {
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(Auditoriums.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Deletes an auditorium by its internal identifier.
        /// </summary>
        /// <param name="AuditoriumId">The unique auditorium identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// Returns <c>200 OK</c> on success, <c>404 Not Found</c> when no auditorium exists for the ID,
        /// or <c>500 Internal Server Error</c> when an unexpected error occurs.
        /// </returns>
        [HttpDelete]
        [Route("{AuditoriumId:int}")]
        public async Task<IActionResult> DeleteById(int AuditoriumId)
        {
            try
            {
                var result = await _AuditoriumService.DeleteAuditoriumByIdAsync(AuditoriumId);
                return result switch
                {
                    { IsFailure: true, Error: "Auditorium not found" } => NotFound($"Auditorium with TmdbId {AuditoriumId} not found"),
                    { IsSuccess: true } => Ok($"Auditorium with tmdbId {AuditoriumId} and title {result.Value} deleted"),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Retrieves an auditorium by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the Auditorium to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the auditorium details when found,
        /// or an error response when not found or when retrieval fails.
        /// </returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAuditoriumById(int id)
        {
            try
            {
                var Auditorium = await _AuditoriumService.GetAuditoriumAsync(id);
                return Auditorium switch
                {
                    { IsFailure: true, Error: "Auditorium not found" } => NotFound(new { error = "Auditorium not found" }),
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(Auditorium.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Creates a new auditorium.
        /// </summary>
        /// <param name="name">The name of the auditorium to create.</param>
        /// <param name="rows">The row and seat configuration for the auditorium.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created auditorium data on success,
        /// or an error response when creation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddAuditoriumById( 
            [FromQuery] string name,
            [FromQuery] List<RowConfig> rows)
        { 
            try
            {
                var result = await _AuditoriumService.AddAuditoriumAsync(new CreateAuditoriumRequest(name, rows));
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}

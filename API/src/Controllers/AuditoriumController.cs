using API.Repositories.Interfaces;
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
        private readonly IAuditoriumRepository _AuditoriumRepository;

        /// <summary>
        /// A controller for managing Auditorium-related operations, providing endpoints to retrieve,
        /// and manage Auditorium data.
        /// </summary>
        public AuditoriumController(IAuditoriumRepository AuditoriumRepository)
        {
            _AuditoriumRepository = AuditoriumRepository;
        }


        /// Retrieves a list of all Auditoriums from the repository.
        /// The method attempts to fetch all Auditoriums using the injected repository.
        /// If the operation fails, it will return an appropriate HTTP status code
        /// indicating the error (e.g., 500 Internal Server Error). If successful,
        /// it returns the list of Auditoriums.
        /// <return>Returns an IActionResult containing a list of Auditoriums on success, or an error message on failure.</return>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var Auditoriums = await _AuditoriumRepository.GetAuditoriumsAsync();
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
        /// Deletes a Auditorium from the system based on its TmdbId.
        /// </summary>
        /// <param name="tmdbId">The unique TmdbId of the Auditorium to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation. Returns:
        /// - <c>200 OK</c> if the Auditorium was successfully deleted.
        /// - <c>404 Not Found</c> if the Auditorium with the specified TmdbId was not found.
        /// - <c>500 Internal Server Error</c> if an unexpected error occurs.
        /// </returns>
        [HttpDelete]
        [Route("{AuditoriumId:int}")]
        public async Task<IActionResult> DeleteById(int AuditoriumId)
        {
            try
            {
                var result = await _AuditoriumRepository.DeleteAuditoriumByIdAsync(AuditoriumId);
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


        /// Retrieves a Auditorium by its unique identifier.
        /// <param name="id">The unique identifier of the Auditorium to retrieve.</param>
        /// <returns>An IActionResult containing the Auditorium details if found, a 404 Not Found response if the Auditorium is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAuditoriumById(int id)
        {
            try
            {
                var Auditorium = await _AuditoriumRepository.GetAuditoriumAsync(id);
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
        /// Adds a Auditorium to the database.
        /// </summary>
        /// <param name="name">The Auditorium name to be added. Must be a positive integer. that does not exist</param>
        /// <param name="rows">The list of row configurations for the Auditorium. Each row configuration should specify the number of seats and wheelchair spaces.</param>
        /// <returns>
        /// Returns a status indicating the result of the operation:
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddAuditoriumById( 
            [FromQuery] string name,
            [FromQuery] List<RowConfig> rows)
        { 
            try
            {
                var result = await _AuditoriumRepository.AddAuditoriumAsync(new CreateAuditoriumRequest(name, rows));
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}


using API.src.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Controllers
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

        /// <summary>
        /// A controller for managing Showing-related operations, providing endpoints to retrieve,
        /// and manage Showing data.
        /// </summary>
        public ShowingController(IShowingRepository ShowingRepository)
        {
            _ShowingRepository = ShowingRepository;
        }


        /// Retrieves a list of all Showings from the repository.
        /// The method attempts to fetch all Showings using the injected repository.
        /// If the operation fails, it will return an appropriate HTTP status code
        /// indicating the error (e.g., 500 Internal Server Error). If successful,
        /// it returns the list of Showings.
        /// <return>Returns an IActionResult containing a list of Showings on success, or an error message on failure.</return>
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

        //[HttpGet]
        //[Route("{ShowingId:int}/state")]
        //public async Task<IActionResult> GetShowingStateById(int ShowingId)
        //{
        //    try
        //    {
        //        var result = await _ShowingRepository.GetShowingStateByIdAsync(ShowingId);
        //        return result switch
        //        {
        //            { IsFailure: true, Error: "Showing not found" } => NotFound(new { error = "Showing not found" }),
        //            { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
        //            { IsSuccess: true } => Ok(result.Value),
        //            _ => StatusCode(500, new { error = "Unexpected result" })
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, new { error = "An error occurred" });
        //    }
        //}

        /// <summary>
        /// Deletes a Showing from the system based on its TmdbId.
        /// </summary>
        /// <param name="tmdbId">The unique TmdbId of the Showing to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation. Returns:
        /// - <c>200 OK</c> if the Showing was successfully deleted.
        /// - <c>404 Not Found</c> if the Showing with the specified TmdbId was not found.
        /// - <c>500 Internal Server Error</c> if an unexpected error occurs.
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
                    { IsFailure: true, Error: "Showing not found" } => NotFound($"Showing with TmdbId {ShowingId} not found"),
                    { IsSuccess: true } => Ok($"Showing with tmdbId {ShowingId} and title {result.Value} deleted"),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }


        /// Retrieves a Showing by its unique identifier.
        /// <param name="id">The unique identifier of the Showing to retrieve.</param>
        /// <returns>An IActionResult containing the Showing details if found, a 404 Not Found response if the Showing is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
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
        /// Adds a Showing to the database.
        /// </summary>
        /// <param name="name">The Showing name to be added. Must be a positive integer. that does not exist</param>
        /// <param name="rows">The list of row configurations for the Showing. Each row configuration should specify the number of seats and wheelchair spaces.</param>
        /// <returns>
        /// Returns a status indicating the result of the operation:
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddShowingById( 
            [FromQuery] int movieId,
            [FromQuery] int auditoriumId,
            [FromQuery] DateTimeOffset startsAt)
        { 
            try
            {
                var result = await _ShowingRepository.AddShowingAsync(new CreateShowingRequest(movieId, auditoriumId, startsAt));
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}

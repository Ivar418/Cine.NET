
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        /// <summary>
        /// Represents the repository responsible for providing data access functionalities
        /// related to Reservations. The repository abstracts interactions with the data sources,
        /// such as databases or external APIs, to perform operations including retrieval,
        /// search, creation, updating, and deletion of Reservation records.
        /// </summary>
        private readonly IReservationService _reservationService;

        /// <summary>
        /// A controller for managing Reservation-related operations, providing endpoints to retrieve,
        /// and manage Reservation data.
        /// </summary>
        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        [Route("update-seats")]
        public async Task<IActionResult> UpdateReservationSeats([FromBody] UpdateReservationSeatsRequest req)
        {
            try
            {
                var result = await _reservationService.UpdateReservationSeatsAsync(req.SuggestionId, req.Seats);
                return result is null ? NotFound(new { error = "Reservation not found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }


        /// Retrieves a Reservation by its unique identifier.
        /// <param reservationId="id">The unique identifier of the Reservation to retrieve.</param>
        /// <returns>An IActionResult containing the Reservation details if found, a 404 Not Found response if the Reservation is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        [Route("{reservationId:guid}")]
        public async Task<IActionResult> GetReservationById(Guid reservationId)
        {
            try
            {
                var Reservation = await _reservationService.GetReservationByIdAsync(reservationId);
                return Reservation switch
                {
                    { IsFailure: true, Error: "Reservation not found" } => NotFound(new { error = "Reservation not found" }),
                    { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                    { IsSuccess: true } => Ok(Reservation.Value),
                    _ => StatusCode(500, new { error = "Unexpected result" })
                };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// Retrieves a Reservation by its unique identifier.
        /// <param reservationId="id">The unique identifier of the Reservation to retrieve.</param>
        /// <returns>An IActionResult containing the Reservation details if found, a 404 Not Found response if the Reservation is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        [Route("showtime/{showingId:int}")]
        public async Task<IActionResult> GetReservationByShowingIdAsync(int showingId)
        {
            try
            {
                var Reservation = await _reservationService.GetReservationByShowingAsync(showingId);
                return Reservation.Count > 0 ? Ok(Reservation) : NotFound(new { error = "No Reservations found for the given showingId" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Adds a Reservation to the database.
        /// </summary>
        /// <param name="showingId">The showingId to be added. Must be a positive integer.</param>
        /// <param name="normalCount">The list of seatInfo for the Reservation.</param>
        /// <param name="wheelchairCount">The Reservation status to be added. Must be a enum of the type ReservationStatus.</param>
        /// <returns>
        /// Returns a status indicating the result of the operation:
        /// </returns>
        [HttpPost]
        [Route("suggest")]
        public async Task<IActionResult> AddReservationById(
            [FromBody] SuggestRequest request)
        {
            try
            {
                var result = await _reservationService.SuggestAsync(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        [HttpPost]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmReservationById(
            [FromBody] ConfirmRequest reservationId)
        {
            try
            {
                var result = await _reservationService.UpdateReservationStatusAsync(reservationId.SuggestionId, "Confirmed");
                return result is null ? NotFound(new { error = "Reservation not found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        [HttpPost]
        [Route("cancel")]
        public async Task<IActionResult> CancelReservationById(
            [FromBody] CancelRequest reservationId)
        {
            try
            {
                var result = await _reservationService.UpdateReservationStatusAsync(reservationId.ReservationId, "Cancelled");
                return result is null ? NotFound(new { error = "Reservation not found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}

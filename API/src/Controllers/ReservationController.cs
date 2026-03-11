
using API.src.Repositories.Interfaces;
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
        private readonly IReservationRepository _ReservationRepository;

        /// <summary>
        /// A controller for managing Reservation-related operations, providing endpoints to retrieve,
        /// and manage Reservation data.
        /// </summary>
        public ReservationController(IReservationRepository ReservationRepository)
        {
            _ReservationRepository = ReservationRepository;
        }


        /// Retrieves a Reservation by its unique identifier.
        /// <param reservationId="id">The unique identifier of the Reservation to retrieve.</param>
        /// <returns>An IActionResult containing the Reservation details if found, a 404 Not Found response if the Reservation is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        [Route("/{reservationId:guid}")]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            try
            {
                var Reservation = await _ReservationRepository.GetReservationByIdAsync(id);
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
        [Route("/showtime/{showingId:guid}")]
        public async Task<IActionResult> GetReservationByShowingIdAsync(int showingId)
        {
            try
            {
                var Reservation = await _ReservationRepository.GetReservationByShowingAsync(showingId);
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
        /// <param name="name">The Reservation name to be added. Must be a positive integer. that does not exist</param>
        /// <param name="rows">The list of row configurations for the Reservation. Each row configuration should specify the number of seats and wheelchair spaces.</param>
        /// <returns>
        /// Returns a status indicating the result of the operation:
        /// </returns>
        [HttpPost]
        [Route("/suggest")]
        public async Task<IActionResult> AddReservationById(
            [FromQuery] int showtimeId,
            [FromQuery] IEnumerable<SeatInfo> seats,
            [FromQuery] string status)
        {
            try
            {
                var result = await _ReservationRepository.CreateReservationAsync(showtimeId, seats, status);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        [HttpPost]
        [Route("/confirm")]
        public async Task<IActionResult> ConfirmReservationById(
            [FromQuery] Guid reservationId)
        {
            try
            {
                var result = await _ReservationRepository.UpdateReservationStatusAsync(reservationId, "Confirmed");
                return result is null ? NotFound(new { error = "Reservation not found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        [HttpPost]
        [Route("/cancel")]
        public async Task<IActionResult> CancelReservationById(
            [FromQuery] Guid reservationId)
        {
            try
            {
                var result = await _ReservationRepository.UpdateReservationStatusAsync(reservationId, "Cancelled");
                return result is null ? NotFound(new { error = "Reservation not found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}

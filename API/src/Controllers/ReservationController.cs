
using API.src.Repositories.Implementations;
using API.src.Repositories.Interfaces;
using API.src.Services.Implementations;
using API.src.Services.Interfaces;
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
        private readonly IReservationService _reservationService;

        /// <summary>
        /// A controller for managing Reservation-related operations, providing endpoints to retrieve,
        /// and manage Reservation data.
        /// </summary>
        public ReservationController(IReservationRepository ReservationRepository)
        {
            _ReservationRepository = ReservationRepository;
            _reservationService = new ReservationService();
        }


        /// Retrieves a Reservation by its unique identifier.
        /// <param reservationId="id">The unique identifier of the Reservation to retrieve.</param>
        /// <returns>An IActionResult containing the Reservation details if found, a 404 Not Found response if the Reservation is not found, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        [Route("/{reservationId:guid}")]
        public async Task<IActionResult> GetReservationById(Guid reservationId)
        {
            try
            {
                var Reservation = await _ReservationRepository.GetReservationByIdAsync(reservationId);
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
        /// <param name="showingId">The showingId to be added. Must be a positive integer.</param>
        /// <param name="seats">The list of seatInfo for the Reservation.</param>
        /// <param name="status">The Reservation status to be added. Must be a enum of the type ReservationStatus.</param>
        /// <returns>
        /// Returns a status indicating the result of the operation:
        /// </returns>
        [HttpPost]
        [Route("/suggest")]
        public async Task<IActionResult> AddReservationById(
            [FromQuery] int showingId,
            [FromQuery] IEnumerable<SeatInfo> seats,
            [FromQuery] string status)
        {
            try
            {
                SuggestRequest request = new SuggestRequest(
                    showingId,
                    seats.Count(s => s.Type == SeatType.Normal),
                    seats.Count(s => s.Type == SeatType.Wheelchair));

                var result = _reservationService.SuggestAsync(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

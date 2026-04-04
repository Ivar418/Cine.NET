using Microsoft.AspNetCore.Mvc;
using API.Mappers;
using API.Services.Interfaces;
using SharedLibrary.DTOs.Requests;


namespace API.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Retrieves all tickets.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing all tickets.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            var response = TicketMapper.ToResponse(tickets);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a single ticket by its identifier.
        /// </summary>
        /// <param name="id">The ticket identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the ticket when found,
        /// or <c>404 Not Found</c> when the ticket does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound();

            var response = TicketMapper.ToResponse(ticket);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves all tickets for a specific showing.
        /// </summary>
        /// <param name="showingId">The showing identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing all matching tickets.
        /// </returns>
        [HttpGet("showing/{showingId}")]
        public async Task<IActionResult> GetByShowingId(int showingId)
        {
            var tickets = await _ticketService.GetShowingTicketsAsync(showingId);
            var response = TicketMapper.ToResponse(tickets);
            return Ok(response);
        }

        /// <summary>
        /// Creates a new ticket.
        /// </summary>
        /// <param name="request">The ticket data to create.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created ticket,
        /// including a route reference to retrieve it.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create(TicketRequest request)
        {
            var ticket = TicketMapper.ToEntity(request);
            var created = await _ticketService.CreateTicketAsync(ticket);
            var response = TicketMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Updates an existing ticket.
        /// </summary>
        /// <param name="id">The ticket identifier.</param>
        /// <param name="request">The updated ticket data.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated ticket when successful,
        /// or <c>404 Not Found</c> when the ticket does not exist.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TicketRequest request)
        {
            var existing = await _ticketService.GetTicketByIdAsync(id);
            if (existing == null)
                return NotFound();

            var ticket = TicketMapper.ToEntity(request);
            ticket.Id = id;
            await _ticketService.UpdateTicketAsync(ticket);
            var response = TicketMapper.ToResponse(ticket);
            return Ok(response);
        }

        /// <summary>
        /// Deletes an existing ticket.
        /// </summary>
        /// <param name="id">The ticket identifier.</param>
        /// <returns>
        /// A <see cref="NoContentResult"/> when successful,
        /// or <c>404 Not Found</c> when the ticket does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound();

            await _ticketService.DeleteTicketAsync(id);
            return NoContent();
        }
    }

}
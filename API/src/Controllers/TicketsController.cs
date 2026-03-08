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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            var response = TicketMapper.ToResponse(tickets);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound();

            var response = TicketMapper.ToResponse(ticket);
            return Ok(response);
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetByMovieId(int movieId)
        {
            var tickets = await _ticketService.GetMovieTicketsAsync(movieId);
            var response = TicketMapper.ToResponse(tickets);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketRequest request)
        {
            var ticket = TicketMapper.ToEntity(request);
            var created = await _ticketService.CreateTicketAsync(ticket);
            var response = TicketMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

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
using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface ITicketService
{
    Task<IReadOnlyList<Ticket>> GetAllTicketsAsync();
    Task<Ticket?> GetTicketByIdAsync(int id);
    Task<IReadOnlyList<Ticket>> GetShowingTicketsAsync(int showingId);
    Task<Ticket> CreateTicketAsync(Ticket ticket);
    Task UpdateTicketAsync(Ticket ticket);
    Task DeleteTicketAsync(int id);
}
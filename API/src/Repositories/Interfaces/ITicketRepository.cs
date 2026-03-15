using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces;

public interface ITicketRepository
{
    Task<IReadOnlyList<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(int id);
    Task<IReadOnlyList<Ticket>> GetTicketsByShowingIdAsync(int showingId);
    Task AddAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
    Task DeleteAsync(int id);

}
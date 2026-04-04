using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;


namespace API.Repositories.Implementations;

public class TicketRepository: ITicketRepository
{
    private readonly ApiDbContext _db;

    public TicketRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all tickets including related showing data.
    /// </summary>
    /// <returns>A read-only list of tickets.</returns>
    public async Task<IReadOnlyList<Ticket>> GetAllAsync()
        => await _db.Tickets.AsNoTracking()
            .Include(t => t.Showing)
            .ToListAsync();

    /// <summary>
    /// Retrieves a ticket by identifier including related showing data.
    /// </summary>
    /// <param name="id">The ticket identifier.</param>
    /// <returns>The matching ticket, or <c>null</c> when not found.</returns>
    public async Task<Ticket?> GetByIdAsync(int id)
        => await _db.Tickets.AsNoTracking()
            .Include(t => t.Showing)
            .FirstOrDefaultAsync(t => t.Id == id);

    /// <summary>
    /// Retrieves all tickets linked to a specific showing.
    /// </summary>
    /// <param name="showingId">The showing identifier.</param>
    /// <returns>A read-only list of tickets for the specified showing.</returns>
    public async Task<IReadOnlyList<Ticket>> GetTicketsByShowingIdAsync(int showingId)
        => await _db.Tickets.AsNoTracking()
            .Where(t => t.ShowingId == showingId)
            .Include(t => t.Showing.Movie)
            .ToListAsync();

    /// <summary>
    /// Persists a new ticket.
    /// </summary>
    /// <param name="ticket">The ticket entity to store.</param>
    public async Task AddAsync(Ticket ticket)
    {
        await _db.Tickets.AddAsync(ticket);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing ticket.
    /// </summary>
    /// <param name="ticket">The ticket entity with updated values.</param>
    public async Task UpdateAsync(Ticket ticket)
    {
        _db.Tickets.Update(ticket);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a ticket by identifier when it exists.
    /// </summary>
    /// <param name="id">The ticket identifier.</param>
    public async Task DeleteAsync(int id)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket != null)
        {
            _db.Tickets.Remove(ticket);
            await _db.SaveChangesAsync();
        }
    }
}

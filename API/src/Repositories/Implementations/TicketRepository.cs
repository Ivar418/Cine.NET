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

    public async Task<IReadOnlyList<Ticket>> GetAllAsync()
        => await _db.Tickets.AsNoTracking()
            .Include(t => t.Showing)
            .ToListAsync();

    public async Task<Ticket?> GetByIdAsync(int id)
        => await _db.Tickets.AsNoTracking()
            .Include(t => t.Showing)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IReadOnlyList<Ticket>> GetTicketsByShowingIdAsync(int showingId)
        => await _db.Tickets.AsNoTracking()
            .Where(ticket => ticket.ShowingId == showingId)
            .Include(ticket => ticket.Showing.Movie)
            .ToListAsync();

    public async Task AddAsync(Ticket ticket)
    {
        await _db.Tickets.AddAsync(ticket);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _db.Tickets.Update(ticket);
        await _db.SaveChangesAsync();
    }

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

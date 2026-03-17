using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;

namespace API.Repositories.Implementations;

public class TicketTypeRepository : ITicketTypeRepository
{
    private readonly ApiDbContext _db;

    public TicketTypeRepository(ApiDbContext db)
    {
        _db = db;
    }

    public async Task<List<TicketType>> GetAllAsync()
    {
        return await _db.TicketTypes
            .AsNoTracking()
            .ToListAsync();
    }
}
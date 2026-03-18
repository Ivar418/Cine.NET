using API.Domain.Common;
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

    public async Task<ResultOf<List<TicketType>>> GetAllAsync()
    {
        try
        {
            var ticketTypes = await _db.TicketTypes
                .AsNoTracking()
                .ToListAsync();

            return ResultOf<List<TicketType>>.Success(ticketTypes);
        }
        catch (Exception ex)
        {
            return ResultOf<List<TicketType>>.Failure(ex.Message);
        }
    }
}
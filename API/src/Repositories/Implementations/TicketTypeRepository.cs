using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;

namespace API.Repositories.Implementations;

/// <summary>
/// Repository for retrieving TicketType entities from the database.
/// </summary>
public class TicketTypeRepository : ITicketTypeRepository
{
    private readonly ApiDbContext _db;

    /// <summary>
    /// Initializes a new instance of the TicketTypeRepository.
    /// </summary>
    /// <param name="db">The database context.</param>
    public TicketTypeRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all TicketTypes from the database.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a list of <see cref="TicketType"/> on success.
    /// Returns a failure result if an error occurs.
    /// </returns>
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
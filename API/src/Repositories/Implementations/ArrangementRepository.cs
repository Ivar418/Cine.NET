using API.Domain.Common;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;

/// <summary>
/// Provides database access for arrangement entities.
/// </summary>
namespace API.Repositories.Implementations;

public class ArrangementRepository : IArrangementRepository
{
    private readonly ApiDbContext _db;

    public ArrangementRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all arrangements including their item collections.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the arrangement list on success,
    /// or a failure result when the query cannot be completed.
    /// </returns>
    public async Task<ResultOf<IReadOnlyList<Arrangement>>> GetAllAsync()
    {
        try
        {
            var arrangements = await _db.Arrangements
                .Include(a => a.Items)
                .AsNoTracking()
                .ToListAsync();

            return ResultOf<IReadOnlyList<Arrangement>>.Success(arrangements);
        }
        catch (Exception ex)
        {
            return ResultOf<IReadOnlyList<Arrangement>>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a single arrangement by identifier including its item collection.
    /// </summary>
    /// <param name="id">The arrangement identifier.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the arrangement when found,
    /// or a failure result when the arrangement does not exist or retrieval fails.
    /// </returns>
    public async Task<ResultOf<Arrangement?>> GetByIdAsync(int id)
    {
        try
        {
            var arrangement = await _db.Arrangements
                .Include(a => a.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (arrangement == null)
                return ResultOf<Arrangement?>.Failure("Arrangement not found");

            return ResultOf<Arrangement?>.Success(arrangement);
        }
        catch (Exception ex)
        {
            return ResultOf<Arrangement?>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Persists a new arrangement and its related items.
    /// </summary>
    /// <param name="arrangement">The arrangement entity to create.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the persisted arrangement on success,
    /// or a failure result when persistence fails.
    /// </returns>
    public async Task<ResultOf<Arrangement>> CreateAsync(Arrangement arrangement)
    {
        try
        {
            _db.Arrangements.Add(arrangement);
            await _db.SaveChangesAsync();

            return ResultOf<Arrangement>.Success(arrangement);
        }
        catch (Exception ex)
        {
            return ResultOf<Arrangement>.Failure(ex.Message);
        }
    }
}
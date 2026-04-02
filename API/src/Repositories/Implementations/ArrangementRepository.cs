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
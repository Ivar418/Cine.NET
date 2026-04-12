using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Repository for retrieving pricing configuration key-value pairs from the database.
/// </summary>
public class PricingConfigRepository : IPricingConfigRepository
{
    private readonly ApiDbContext _db;

    /// <summary>
    /// Initializes a new instance of the PricingConfigRepository.
    /// </summary>
    /// <param name="db">The database context.</param>
    public PricingConfigRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all pricing configuration entries as key-value pairs.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a dictionary of configuration keys and values on success.
    /// Returns a failure result if an error occurs.
    /// </returns>
    public async Task<ResultOf<Dictionary<string, decimal>>> GetAllAsync()
    {
        try
        {
            var config = await _db.PricingConfigs
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Key, x => x.Value);

            return ResultOf<Dictionary<string, decimal>>.Success(config);
        }
        catch (Exception ex)
        {
            return ResultOf<Dictionary<string, decimal>>.Failure(ex.Message);
        }
    }
}
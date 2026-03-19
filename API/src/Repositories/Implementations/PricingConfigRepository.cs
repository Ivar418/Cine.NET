using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class PricingConfigRepository : IPricingConfigRepository
{
    private readonly ApiDbContext _db;

    public PricingConfigRepository(ApiDbContext db)
    {
        _db = db;
    }

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
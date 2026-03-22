using API.Domain.Common;

namespace API.Repositories.Interfaces;

public interface IPricingConfigRepository
{
    Task<ResultOf<Dictionary<string, decimal>>> GetAllAsync();
}
using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

public class PricingService : IPricingService
{
    private readonly IPricingConfigRepository _repo;
    private Dictionary<string, decimal>? _config;

    public PricingService(IPricingConfigRepository repo)
    {
        _repo = repo;
    }

    private async Task<ResultOf<Dictionary<string, decimal>>> GetConfigAsync()
    {
        if (_config != null)
            return ResultOf<Dictionary<string, decimal>>.Success(_config);

        var result = await _repo.GetAllAsync();

        if (result.IsFailure)
            return ResultOf<Dictionary<string, decimal>>.Failure(result.Error!);

        _config = result.Value!;
        return result;
    }

    public async Task<ResultOf<decimal>> CalculatePriceAsync(Movie movie, bool isThreeD, TicketType ticketType)
    {
        var configResult = await GetConfigAsync();

        if (configResult.IsFailure)
            return ResultOf<decimal>.Failure(configResult.Error!);

        var config = configResult.Value!;

        var price = movie.Runtime > 120
            ? config["LongMoviePrice"]
            : config["BasePrice"];

        if (isThreeD)
            price += config["ThreeDSurcharge"];

        price -= ticketType.Discount;

        return ResultOf<decimal>.Success(price);
    }
}
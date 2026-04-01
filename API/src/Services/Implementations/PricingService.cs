using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

/// <summary>
/// Service responsible for calculating ticket prices based on configuration and business rules.
/// </summary>
public class PricingService : IPricingService
{
    private readonly IPricingConfigRepository _repo;
    private Dictionary<string, decimal>? _config;

    /// <summary>
    /// Initializes a new instance of the PricingService.
    /// </summary>
    /// <param name="repo">The pricing configuration repository.</param>
    public PricingService(IPricingConfigRepository repo)
    {
        _repo = repo;
    }

    
    /// <summary>
    /// Retrieves pricing configuration, using in-memory cache if available.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing configuration key-value pairs on success.
    /// Returns a failure result if configuration cannot be retrieved.
    /// </returns>
    private async Task<ResultOf<Dictionary<string, decimal>>> GetConfigAsync()
    {
        if (_config != null)
            return ResultOf<Dictionary<string, decimal>>.Success(_config);

        var result = await _repo.GetAllAsync();

        if (result.IsFailure)
            return ResultOf<Dictionary<string, decimal>>.Failure(result.Error!);

        _config = result.Value!;
        return ResultOf<Dictionary<string, decimal>>.Success(_config);
    }
    
    /// <summary>
    /// Calculates the ticket price based on movie runtime, 3D status, and ticket type.
    /// </summary>
    /// <param name="movie">The movie entity.</param>
    /// <param name="isThreeD">Indicates whether the showing is in 3D.</param>
    /// <param name="ticketType">The ticket type.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the calculated price on success.
    /// Returns a failure result if configuration cannot be retrieved.
    /// </returns>
    public async Task<ResultOf<decimal>> CalculatePriceAsync(
        Movie movie,
        bool isThreeD,
        TicketType ticketType)
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
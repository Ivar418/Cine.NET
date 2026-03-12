using API.Infrastructure.Database;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

public class PricingService
{
    private readonly Dictionary<string, decimal> _config;

    public PricingService(ApiDbContext db)
    {
        _config = db.PricingConfigs
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public decimal CalculatePrice(Movie movie, bool isThreeD, TicketType ticketType)
    {
        var price = movie.Runtime > 120
            ? _config["LongMoviePrice"]
            : _config["BasePrice"];

        if (isThreeD)
            price += _config["ThreeDSurcharge"];

        price -= ticketType.Discount;

        return price;
    }
}
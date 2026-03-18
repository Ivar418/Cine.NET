using API.Domain.Common;
using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface IPricingService
{
    Task<ResultOf<decimal>> CalculatePriceAsync(
        Movie movie,
        bool isThreeD,
        TicketType ticketType);
}
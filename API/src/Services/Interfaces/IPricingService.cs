using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface IPricingService
{
    decimal CalculatePrice(Movie movie, bool isThreeD, TicketType ticketType);
}
namespace API.Services.Interfaces;

using SharedLibrary.Domain.Entities;

public interface ITicketRuleService
{
    bool IsTicketTypeAvailable(TicketType type, Showing showing, DateTime now);
}
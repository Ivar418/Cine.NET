using API.Domain.Common;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

public class TicketRuleService : ITicketRuleService
{
    
    public bool IsTicketTypeAvailable(TicketType type, Showing showing, DateTime now)
    {
        var isWeekday = DateTimeHelper.IsMaDiWoDo(now);
        var isBefore18Hour = showing.StartsAt.TimeOfDay < TimeSpan.FromHours(18);

        switch (type.Name)
        {
            case "Child":
                return isBefore18Hour;
            // && showing.Movie.AgeRating < 16 later toevoegen indien nodig

            case "Student":
                return isWeekday;

            case "Senior":
                return isWeekday && !DateTimeHelper.IsHoliday(now);

            case "Adult":
                return true;

            default:
                return false;
        }
    }
}
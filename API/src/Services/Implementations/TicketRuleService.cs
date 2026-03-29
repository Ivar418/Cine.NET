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
        var isBelowAgeIndication = 
            int.TryParse(showing.Movie.AgeIndication, out var age) && age < 12;
        var isDutchSpokenLanguage = 
            string.Equals(showing.Movie.SpokenLanguageCodeIso6391, "nl", StringComparison.OrdinalIgnoreCase);

        switch (type.Name)
        {
            case "Child":
                return isBefore18Hour &&
                       isBelowAgeIndication &&
                       isDutchSpokenLanguage;
            
            case "Student":
                return isWeekday;

            case "Senior":
                return isWeekday && 
                       !DateTimeHelper.IsHoliday(now);

            case "Adult":
                return true;

            default:
                return false;
        }
    }
}
namespace API.Domain.Common;

// Helper methods for reusable date/time checks (e.g. weekday-based rules like Mon–Thu discounts)

public static class DateTimeHelper
{
    public static bool IsMaDiWoDo(DateTime date)
    {
        return date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Thursday;
    }
}
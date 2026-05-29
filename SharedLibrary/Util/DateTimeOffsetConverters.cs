namespace SharedLibrary.Util;

public class DateTimeOffsetConverters
{
    // --- Helper method to validate UTC timestamp ---
    private static bool IsValidUtcTimestamp(string timestamp)
    {
        return DateTimeOffset.TryParseExact(
            timestamp,
            "O", // Exact ISO 8601 round-trip format
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out var dto
        ) && dto.Offset == TimeSpan.Zero; // Must be +00:00
    }

    public static DateTimeOffset StringToDateTimeOffsetConverter(string timestamp)
    {
        if (DateTimeOffset.TryParseExact(
                timestamp,
                "O",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result))
        {
            return result;
        }

        throw new FormatException("The timestamp is not a valid ISO 8601 timestamp.");
    }
}
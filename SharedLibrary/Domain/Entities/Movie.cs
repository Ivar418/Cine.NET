using System.ComponentModel.DataAnnotations.Schema;
using SharedLibrary.DTOs.Responses.TMDB;

namespace SharedLibrary.Domain.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TmdbId { get; set; } = 0;
    public string? Language { get; set; }
    public string? PosterUrl { get; set; }
    public int? Runtime { get; set; }
    public string? ImdbId { get; set; }
    public string? ReleaseDate { get; set; }
    public string? About { get; set; }
    public string? AgeIndication { get; set; }
    public string? SpokenLanguageName { get; set; }
    public string? SpokenLanguageCodeIso6391 { get; set; }

    public List<int>? GenresIds { get; set; }


    // --- Row Timestamps ---
    private string _rowCreatedTimestampUtc = string.Empty;
    private string? _rowUpdatedTimestampUtc;
    private string? _rowDeletedTimestampUtc;

    [Column("row_created_timestamp_utc")]
    public string RowCreatedTimestampUtc
    {
        get => _rowCreatedTimestampUtc;
        init
        {
            if (!IsValidUtcTimestamp(value))
                throw new ArgumentException("RowCreatedTimestampUtc must be a valid UTC timestamp with +00:00 offset.");
            _rowCreatedTimestampUtc = value;
        }
    }

    [Column("row_updated_timestamp_utc")]
    public string? RowUpdatedTimestampUtc
    {
        get => _rowUpdatedTimestampUtc;
        set
        {
            if (value != null && !IsValidUtcTimestamp(value))
                throw new ArgumentException("RowUpdatedTimestampUtc must be a valid UTC timestamp with +00:00 offset.");
            _rowUpdatedTimestampUtc = value;
        }
    }

    [Column("row_deleted_timestamp_utc")]
    public string? RowDeletedTimestampUtc
    {
        get => _rowDeletedTimestampUtc;
        set
        {
            if (value != null && !IsValidUtcTimestamp(value))
                throw new ArgumentException("RowDeletedTimestampUtc must be a valid UTC timestamp with +00:00 offset.");
            _rowDeletedTimestampUtc = value;
        }
    }

    // --- Helper method to validate UTC timestamp ---
    private static bool IsValidUtcTimestamp(string timestamp)
    {
        if (DateTimeOffset.TryParse(timestamp, out var dto))
        {
            // Must be UTC offset +00:00
            return dto.Offset == TimeSpan.Zero;
        }

        return false;
    }

    // --- Helper factory method to generate current UTC string ---
    public static string CurrentUtcTimestamp() => DateTimeOffset.UtcNow.ToString("O");
}
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public DateTime ShowDateTime { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = "Active"; // Active, Used, Cancelled, Expired
    public DateTime PurchaseDate { get; set; }
    

    // Navigation
    public Movie? Movie { get; set; }


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

    // --- Public constructor ---
    public Ticket(int movieId, DateTime showDateTime, string seatNumber, decimal price)
    {
        MovieId = movieId;
        ShowDateTime = showDateTime;
        SeatNumber = seatNumber;
        Price = price;
        Status = "Active";
        PurchaseDate = DateTime.UtcNow;
        RowCreatedTimestampUtc = CurrentUtcTimestamp();
    }

    // --- Business logic ---
    public void MarkAsUsed()
    {
        if (Status != "Active")
            throw new InvalidOperationException("Only active tickets can be marked as used.");

        Status = "Used";
        RowUpdatedTimestampUtc = CurrentUtcTimestamp();
    }

    public void Cancel()
    {
        if (Status != "Active")
            throw new InvalidOperationException("Only active tickets can be cancelled.");

        Status = "Cancelled";
        RowUpdatedTimestampUtc = CurrentUtcTimestamp();
    }

    public void Expire()
    {
        if (Status != "Active")
            throw new InvalidOperationException("Only active tickets can expire.");

        Status = "Expired";
        RowUpdatedTimestampUtc = CurrentUtcTimestamp();
    }

    public bool IsActive() => Status == "Active";
    public bool IsUsed() => Status == "Used";
    public bool IsCancelled() => Status == "Cancelled";
    public bool IsExpired() => Status == "Expired";

    // --- Validation UTC timestamp ---
    private static bool IsValidUtcTimestamp(string timestamp)
    {
        if (DateTimeOffset.TryParse(timestamp, out var dto))
        {
            return dto.Offset == TimeSpan.Zero;
        }

        return false;
    }

    // --- Current UTC timestamp ---
    public static string CurrentUtcTimestamp() => DateTimeOffset.UtcNow.ToString("O");
}
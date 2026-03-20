using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

public class Ticket
{
    [Column("id")] public int Id { get; set; }
    [Column("showing_id")] public int ShowingId { get; set; }

    [Column("shown_date_time_utc")]
    public string ShowDateTimeUtc
    { get => _showDateTimeUtcUtc; init { if (!IsValidUtcTimestamp(value))
                throw new ArgumentException("ShowDateTimeUtc must be a valid UTC timestamp with +00:00 offset.");
            _showDateTimeUtcUtc = value;
        }
    }

    [Column("seat_number")] public string SeatNumber { get; set; } = string.Empty;
    [Column("ticket_type")] public string TicketType { get; set; } = string.Empty;
    [Column("price")] public decimal Price { get; set; }
    [Column("status")] public string Status { get; set; } = "Active"; // Active, Used, Cancelled, Expired

    [Column("payment_status")]
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed, Cancelled

    [Column("qr_code_guid")] public string? QrCodeGuid { get; set; } = string.Empty;
    [Column("qr_is_active")] public bool QrIsActive { get; set; } = false;
    [Column("purchase_date")] public DateTimeOffset PurchaseDate { get; set; }
    public Showing? Showing { get; set; }

    // --- Row Timestamps ---
    private string _rowCreatedTimestampUtc = string.Empty;
    private string? _rowUpdatedTimestampUtc;
    private string? _rowDeletedTimestampUtc;

    private string _showDateTimeUtcUtc = string.Empty;

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
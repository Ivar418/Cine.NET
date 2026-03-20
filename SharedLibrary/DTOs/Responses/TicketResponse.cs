namespace SharedLibrary.DTOs.Responses;

public class TicketResponse
{
    public int Id { get; set; }
    public int ShowingId { get; set; }
    public string MovieTitle { get; set; } = null!;
    public DateTimeOffset ShowDateTime { get; set; }
    public string SeatNumber { get; set; } = null!;
    public string TicketType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string? QrCodeGuid { get; set; }
    public bool QrIsActive { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset PurchaseDate { get; set; }

}
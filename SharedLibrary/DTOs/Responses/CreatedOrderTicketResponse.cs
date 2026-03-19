namespace SharedLibrary.DTOs.Responses;

public class CreatedOrderTicketResponse
{
    public int TicketId { get; set; }
    public int ShowingId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string TicketType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? TicketCode { get; set; }
}


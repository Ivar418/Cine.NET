namespace SharedLibrary.DTOs.Responses;

public class TicketPdfResponse
{
    public int TicketId { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string OrderCode { get; set; } = string.Empty;
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime ShowDateTime { get; set; }
    public string AuditoriumName { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public string TicketType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public bool QrIsActive { get; set; }
}


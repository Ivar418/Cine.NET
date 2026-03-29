namespace SharedLibrary.DTOs.Responses;

public class OrderPdfResponse
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime ShowDateTime { get; set; }
    public string AuditoriumName { get; set; } = string.Empty;
    public List<TicketPdfResponse> Tickets { get; set; } = [];
}


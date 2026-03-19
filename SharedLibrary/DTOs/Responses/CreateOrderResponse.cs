namespace SharedLibrary.DTOs.Responses;

public class CreateOrderResponse
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<CreatedOrderTicketResponse> Tickets { get; set; } = [];
}


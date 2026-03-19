namespace SharedLibrary.DTOs.Responses;

public class CreateOrderResponse
{
    public int OrderId { get; set; }
    public required string OrderCode { get; set; }
    public required string OrderType { get; set; }
    public required string PaymentStatus { get; set; }
    public required string PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<CreatedOrderTicketResponse> Tickets { get; set; } = [];
}

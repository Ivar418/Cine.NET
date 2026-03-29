namespace SharedLibrary.DTOs.Requests;

public class CreateOrderRequest
{
    public required string OrderType { get; set; }
    public required string PaymentMethod { get; set; }
    public required List<TicketRequest> Tickets { get; set; }
}

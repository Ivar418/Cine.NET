namespace SharedLibrary.DTOs.Requests;

public class CreateOrderRequest
{
    public string OrderType { get; set; } = "Reservation";
    public string PaymentMethod { get; set; } = "Unknown";
    public List<TicketRequest> Tickets { get; set; } = [];
}


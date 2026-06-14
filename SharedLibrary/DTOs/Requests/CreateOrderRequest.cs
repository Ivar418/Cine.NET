using SharedLibrary.Domain.Entities;
using SharedLibrary.Domain.Entities.Enums;

namespace SharedLibrary.DTOs.Requests;

public class CreateOrderRequest {
    public required OrderTypes OrderType { get; set; }
    public required PaymentMethods PaymentMethod { get; set; }
    public required List<TicketRequest> Tickets { get; set; }
    public int? UserId { get; set; }
}
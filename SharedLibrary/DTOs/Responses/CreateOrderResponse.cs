using SharedLibrary.Domain.Entities;
using SharedLibrary.Domain.Entities.Enums;

namespace SharedLibrary.DTOs.Responses;

public class CreateOrderResponse {
    public int OrderId { get; set; }
    public required string OrderCode { get; set; }
    public required OrderTypes OrderType { get; set; }
    public required PaymentStatuses PaymentStatuses { get; set; }
    public required PaymentMethods PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<CreatedOrderTicketResponse> Tickets { get; set; } = [];
    public int? UserId { get; init; } = null;
}
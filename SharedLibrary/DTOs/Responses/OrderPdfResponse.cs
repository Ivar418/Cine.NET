using SharedLibrary.Domain.Entities;
using SharedLibrary.Domain.Entities.Enums;

namespace SharedLibrary.DTOs.Responses;

public class OrderPdfResponse {
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public OrderTypes OrderType { get; set; }
    public PaymentStatuses PaymentStatuses { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime ShowDateTime { get; set; }
    public string AuditoriumName { get; set; } = string.Empty;
    public List<TicketPdfResponse> Tickets { get; set; } = [];
}
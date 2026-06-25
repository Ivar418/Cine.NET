using SharedLibrary.Domain.Entities;
using SharedLibrary.Domain.Entities.Enums;

namespace SharedLibrary.DTOs.Responses;

public class CreatedOrderTicketResponse
{
    public int TicketId { get; set; }
    public int ShowingId { get; set; }
    public required string SeatNumber { get; set; }
    public required string TicketType { get; set; }
    public decimal Price { get; set; }
    public required PaymentStatuses PaymentStatus { get; set; }
    public string? TicketCode { get; set; }
}

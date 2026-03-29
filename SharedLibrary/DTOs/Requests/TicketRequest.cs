namespace SharedLibrary.DTOs.Requests;

public class TicketRequest
{
    public int ShowingId { get; set; }
    public DateTimeOffset ShowDateTimeUtc { get; set; }
    public string SeatNumber { get; set; } = null!;
    public string TicketType { get; set; } = null!;
    public decimal Price { get; set; }

}
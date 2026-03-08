namespace SharedLibrary.DTOs.Requests;

public class TicketRequest
{
    public int MovieId { get; set; }
    public DateTime ShowDateTime { get; set; }
    public string SeatNumber { get; set; } = null!;
    public decimal Price { get; set; }

}
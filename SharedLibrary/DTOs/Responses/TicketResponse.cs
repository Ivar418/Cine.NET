namespace SharedLibrary.DTOs.Responses;

public class TicketResponse
{
    public int Id { get; set; }
    public int ShowingId { get; set; }
    public string MovieTitle { get; set; } = null!;
    public DateTime ShowDateTime { get; set; }
    public string SeatNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; }

}
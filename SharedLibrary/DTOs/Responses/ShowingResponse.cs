namespace SharedLibrary.DTOs.Responses;

public class ShowingResponse
{
    public int ShowingId { get; set; }
    public string MovieTitle { get; set; } = "";
    public DateTimeOffset StartsAt { get; set; }
    public ShowingPricesResponse Prices { get; set; } = new();
}
namespace SharedLibrary.DTOs.Responses;

public class ShowingPricesResponse
{
    public TicketPriceResponse Adult { get; set; } = default!;
    public TicketPriceResponse Child { get; set; } = default!;
    public TicketPriceResponse Student { get; set; } = default!;
    public TicketPriceResponse Senior { get; set; } = default!;
}
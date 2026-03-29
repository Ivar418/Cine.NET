namespace SharedLibrary.Domain.Entities;

public class OrderTicket
{
    public int OrderId { get; set; }
    public int TicketId { get; set; }

    public Order Order { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
}


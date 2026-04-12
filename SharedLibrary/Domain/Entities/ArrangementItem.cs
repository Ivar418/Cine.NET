namespace SharedLibrary.Domain.Entities;

/// <summary>
/// Represents a single item within an arrangement, such as a ticket, drink, or food.
/// These items are used for descriptive purposes and are not directly processed in orders.
/// </summary>
public class ArrangementItem
{
    public int Id { get; set; }

    public int ArrangementId { get; set; }
    public Arrangement Arrangement { get; set; } = null!;

    public ArrangementItemType Type { get; set; } // Ticket | Drink | Food

    public string Name { get; set; } = null!; // "Cola", "Popcorn", "Ticket"
    public int Quantity { get; set; } = 1;
}
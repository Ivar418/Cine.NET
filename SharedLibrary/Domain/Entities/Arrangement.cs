namespace SharedLibrary.Domain.Entities;

/// <summary>
/// Represents a purchasable arrangement (bundle) that can be selected during checkout.
/// An arrangement contains a fixed price and a collection of items that describe its contents.
/// </summary>
public class Arrangement
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ArrangementItem> Items { get; set; } = new List<ArrangementItem>();
}
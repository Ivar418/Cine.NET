using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

/// <summary>
/// Represents a pricing option that modifies the base ticket price.
/// Used for additional adjustments such as surcharges or discounts.
/// </summary>
public class PricingOption
{
    /// <summary>
    /// The unique identifier of the pricing option.
    /// </summary>
    [Column("id")] public int Id { get; set; }

    /// <summary>
    /// The name of the pricing option (e.g., "3D", "VIP", "Discount").
    /// </summary>
    [Column("name")] public string Name { get; set; }

    /// <summary>
    /// The price modifier applied to the base price.
    /// Can be positive (surcharge) or negative (discount).
    /// </summary>
    [Column("price_modifier")] public decimal PriceModifier { get; set; }
}
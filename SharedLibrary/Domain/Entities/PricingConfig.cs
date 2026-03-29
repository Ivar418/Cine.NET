using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

/// <summary>
/// Represents a pricing configuration entry stored as a key-value pair.
/// Used to define dynamic pricing rules such as base price, surcharges, and discounts.
/// </summary>
public class PricingConfig
{
    /// <summary>
    /// The unique identifier of the configuration entry.
    /// </summary>
    [Column("id")] public int Id { get; set; }

    /// <summary>
    /// The configuration key (e.g., "BasePrice", "ThreeDSurcharge").
    /// </summary>
    [Column("key")] public string Key { get; set; }

    /// <summary>
    /// The numeric value associated with the configuration key.
    /// </summary>
    [Column("value")] public decimal Value { get; set; }
}
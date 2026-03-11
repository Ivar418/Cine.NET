using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

public class PricingOption
{
    [Column("id")] public int Id { get; set; }
    [Column("name")] public string Name { get; set; }
    [Column("price_modifier")] public decimal PriceModifier { get; set; }
}
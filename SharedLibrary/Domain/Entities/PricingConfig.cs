using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

public class PricingConfig
{
    [Column("id")] public int Id { get; set; }
    [Column("key")] public string Key { get; set; }
    [Column("value")] public decimal Value { get; set; }
}
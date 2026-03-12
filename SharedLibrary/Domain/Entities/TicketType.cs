using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

public class TicketType
{
    [Column("id")] public int Id { get; set; }
    [Column("name")] public string Name { get; set; }
    [Column("discount")] public decimal Discount { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

/// <summary>
/// Represents a ticket type with an associated discount applied to the base price.
/// </summary>
public class TicketType
{
    /// <summary>
    /// The unique identifier of the ticket type.
    /// </summary>
    [Column("id")] public int Id { get; set; }

    /// <summary>
    /// The name of the ticket type (e.g., "Adult", "Child", "Student", "Senior").
    /// </summary>
    [Column("name")] public string Name { get; set; }

    /// <summary>
    /// The discount amount applied to the base ticket price.
    /// </summary>
    [Column("discount")] public decimal Discount { get; set; }
}
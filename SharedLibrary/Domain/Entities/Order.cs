namespace SharedLibrary.Domain.Entities;

using System;
using System.Collections.Generic;

public class Order
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string OrderType { get; set; } = "Reservation";
    public string PaymentStatus { get; set; } = "Pending";
    public string PaymentMethod { get; set; } = "Unknown";
    public PaymentMethod? PaymentMethodNavigation { get; set; }
    public int? CashierEmployeeId { get; set; }
    public bool IsPrinted { get; set; } = false;

    public ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();
}

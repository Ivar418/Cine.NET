using SharedLibrary.Domain.Entities.Enums;

namespace SharedLibrary.Domain.Entities;

using System;
using System.Collections.Generic;

public class Order {
    public int Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderTypes OrderType { get; set; } = OrderTypes.Reservation;
    public PaymentStatuses PaymentStatuses { get; set; } = PaymentStatuses.Pending;
    public PaymentMethods PaymentMethod { get; set; } = PaymentMethods.Unknown;
    public int? CashierEmployeeId { get; set; }
    public bool IsPrinted { get; set; } = false;
    public int? UserId { get; init; }
    public User? User { get; init; }
    public ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();
}
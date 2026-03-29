namespace SharedLibrary.Domain.Entities;

using System.Collections.Generic;

public class PaymentMethod
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}


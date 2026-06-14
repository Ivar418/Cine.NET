using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Mappers;

public class OrderMapper {
    public static CreateOrderResponse ToResponse(Order order) {
        return new CreateOrderResponse {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatuses = order.PaymentStatuses,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            UserId = order.UserId,
        };
    }
}
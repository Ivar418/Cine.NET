using API.Domain.Common;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces;

public interface IOrderService
{
    Task<ResultOf<CreateOrderResponse>> CreateAsync(CreateOrderRequest request);
    Task<ResultOf<CreateOrderResponse>> ConfirmPaymentAsync(int orderId);
    Task<ResultOf<CreateOrderResponse>> GetByIdAsync(int orderId);
    Task<ResultOf<CreateOrderResponse>> ResetToPendingAsync(int orderId);
}

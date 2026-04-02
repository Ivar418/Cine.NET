using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;
namespace WA.Services.Http.Interfaces;
public interface IOrderApi
{
    Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest request);
    Task<CreateOrderResponse?> GetOrderAsync(int orderId);
    Task<CreateOrderResponse?> ConfirmPaymentAsync(int orderId);
    Task<byte[]?> GetReservationPdfAsync(int orderId);
    Task<byte[]?> GetTicketsPdfAsync(int orderId);
}

using System.Net.Http.Json;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;
using WA.Services.Http.Interfaces;

namespace WA.Services.Http;

public class OrderApi : IOrderApi
{
    private readonly HttpClient _http;
    private const string BasePath = "api/orders";

    public OrderApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CreateOrderResponse>> GetAllOrdersAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<List<CreateOrderResponse>>(BasePath);
            return result ?? [];
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[OrderApi] GetAllOrders failed: {ex.Message}");
            return [];
        }
    }

    public async Task<CreateOrderResponse?> GetOrderAsync(int orderId)
    {
        try
        {
            return await _http.GetFromJsonAsync<CreateOrderResponse>($"{BasePath}/{orderId}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[OrderApi] GetOrder({orderId}) failed: {ex.Message}");
            return null;
        }
    }

    public async Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(BasePath, request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[OrderApi] CreateOrder failed: {ex.Message}");
            return null;
        }
    }

    public async Task<CreateOrderResponse?> ConfirmPaymentAsync(int orderId)
    {
        try
        {
            var response = await _http.PostAsync($"{BasePath}/{orderId}/confirm-payment", null);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[OrderApi] ConfirmPayment({orderId}) failed: {ex.Message}");
            return null;
        }
    }

    public async Task<byte[]?> GetReservationPdfAsync(int orderId)
    {
        try
        {
            var response = await _http.GetAsync($"{BasePath}/{orderId}/reservation-pdf");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[OrderApi] GetReservationPdf({orderId}) failed: {ex.Message}");
            return null;
        }
    }

    public async Task<byte[]?> GetTicketsPdfAsync(int orderId)
    {
        try
        {
            var response = await _http.GetAsync($"{BasePath}/{orderId}/tickets-pdf");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[OrderApi] GetTicketsPdf({orderId}) failed: {ex.Message}");
            return null;
        }
    }
}


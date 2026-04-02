using System.Net.Http.Json;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;
using WA.Services.Http.Interfaces;

namespace WA.Services.Http;

public class OrderApi : IOrderApi
{
    private readonly HttpClient _http;
    private const string Base = "api/orders";

    public OrderApi(HttpClient http) => _http = http;
    public async Task<List<CreateOrderResponse>> GetAllOrdersAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<List<CreateOrderResponse>>(Base);
            return result ?? [];
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[OrderApi] GetAllOrders failed: {ex.Message}");
            return [];
        }
    }


    public async Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            var r = await _http.PostAsJsonAsync(Base, request);
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<CreateOrderResponse>() : null;
        }
        catch (Exception ex) { Console.Error.WriteLine("[OrderApi] Create: " + ex.Message); return null; }
    }

    public async Task<CreateOrderResponse?> GetOrderAsync(int orderId)
    {
        try { return await _http.GetFromJsonAsync<CreateOrderResponse>(Base + "/" + orderId); }
        catch (Exception ex) { Console.Error.WriteLine("[OrderApi] Get: " + ex.Message); return null; }
    }

    public async Task<CreateOrderResponse?> ConfirmPaymentAsync(int orderId)
    {
        try
        {
            var r = await _http.PostAsync(Base + "/" + orderId + "/confirm-payment", null);
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<CreateOrderResponse>() : null;
        }
        catch (Exception ex) { Console.Error.WriteLine("[OrderApi] Confirm: " + ex.Message); return null; }
    }

    public async Task<byte[]?> GetReservationPdfAsync(int orderId)
    {
        try
        {
            var r = await _http.GetAsync(Base + "/" + orderId + "/reservation-pdf");
            return r.IsSuccessStatusCode ? await r.Content.ReadAsByteArrayAsync() : null;
        }
        catch (Exception ex) { Console.Error.WriteLine("[OrderApi] ResPdf: " + ex.Message); return null; }
    }

    public async Task<byte[]?> GetTicketsPdfAsync(int orderId)
    {
        try
        {
            var r = await _http.GetAsync(Base + "/" + orderId + "/tickets-pdf");
            return r.IsSuccessStatusCode ? await r.Content.ReadAsByteArrayAsync() : null;
        }
        catch (Exception ex) { Console.Error.WriteLine("[OrderApi] TktPdf: " + ex.Message); return null; }
    }
}

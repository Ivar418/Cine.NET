using WA.Services.Http.Interfaces;
using System.Net.Http.Json;

namespace WA.Services.Http.Implementation;

public class MailApi : IMailApi
{
    private readonly HttpClient _http;

    public MailApi(HttpClient http) => _http = http;

    public async Task<(bool Success, string? Error)> SubscribeAsync(string email)
    {
        try
        {
            var url = $"api/mail/subscription/subscribe?email={Uri.EscapeDataString(email)}";
            var res = await _http.PostAsync(url, null); // no body

            if (res.IsSuccessStatusCode)
                return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, error);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task UnsubscribeAsync(string email)
    {
        var res = await _http.DeleteAsync($"api/mail/subscription/unsubscribe?email={Uri.EscapeDataString(email)}");

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    public async Task SendOrderConfirmationAsync(int orderId)
    {
        throw new NotImplementedException();
        var res = await _http.PostAsJsonAsync($"api/mail/order-confirmation/{orderId}", new { });

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    public async Task SendToSubscribedAsync(string subject, string fromName, string emailContent)
    {
        var res = await _http.PostAsJsonAsync("api/mail/subscription/send", new { subject, fromName, emailContent });

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }
}
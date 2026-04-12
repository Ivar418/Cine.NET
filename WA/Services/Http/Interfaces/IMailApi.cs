namespace WA.Services.Http.Interfaces;

public interface IMailApi
{
    Task<(bool Success, string? Error)> SubscribeAsync(string email);
    Task UnsubscribeAsync(string email);
    Task SendOrderConfirmationAsync(int orderId);
    Task SendToSubscribedAsync(string subject, string fromName, string emailContent);
}
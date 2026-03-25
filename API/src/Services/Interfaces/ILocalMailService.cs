using API.Domain.Model;
using MimeKit;

namespace API.Services.Interfaces;

public interface ILocalMailService
{
    public Task<bool> SendEmailToSubscribersAsync(TextPart emailContent, string fromName, string subject);
    public Task<bool> AddAsync(string email);
    public Task<bool> RemoveAsync(string email);
    public Task<bool> ExistsAsync(string email);
    public Task<IEnumerable<EmailSubscription>> GetAllAsync();
}
using API.Domain.Model;

namespace API.Repositories.Interfaces;

public interface IMailSubscriptionRepository
{
    public Task<bool> AddAsync(string email);
    public Task<bool> RemoveAsync(string email);
    public Task<bool> ExistsAsync(string email);
    public Task<IEnumerable<EmailSubscription>> GetAllAsync();
}
using API.Domain.Model;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MailSubscriptionRepository : IMailSubscriptionRepository
{
    private readonly ApiDbContext _db;

    public MailSubscriptionRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Adds a new email subscription.
    /// </summary>
    /// <param name="email">The email address to subscribe.</param>
    /// <returns><c>true</c> when the subscription is stored.</returns>
    public async Task<bool> AddAsync(string email)
    {
        _db.EmailSubscriptions.Add(new EmailSubscription { Email = email });
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Removes an existing email subscription.
    /// </summary>
    /// <param name="email">The email address to unsubscribe.</param>
    /// <returns><c>true</c> when a subscription was removed; otherwise <c>false</c>.</returns>
    public async Task<bool> RemoveAsync(string email)
    {
        var emailEntity = await _db.EmailSubscriptions.Where(e => e.Email == email).FirstOrDefaultAsync();
        if (emailEntity == null)
            return false;
        _db.EmailSubscriptions.Remove(emailEntity);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Checks whether an email address is subscribed.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns><c>true</c> when the email exists in subscriptions; otherwise <c>false</c>.</returns>
    public async Task<bool> ExistsAsync(string email)
    {
        if (await _db.EmailSubscriptions.AnyAsync(e => e.Email == email))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves all email subscriptions.
    /// </summary>
    /// <returns>A collection of subscribed email entries.</returns>
    public async Task<IEnumerable<EmailSubscription>> GetAllAsync()
    {
        return await _db.EmailSubscriptions.ToListAsync();
    }
}
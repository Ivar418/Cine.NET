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

    public async Task<bool> AddAsync(string email)
    {
        _db.EmailSubscriptions.Add(new EmailSubscription { Email = email });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(string email)
    {
        var emailEntity = await _db.EmailSubscriptions.Where(e => e.Email == email).FirstOrDefaultAsync();
        if (emailEntity == null)
            return false;
        _db.EmailSubscriptions.Remove(emailEntity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        if (await _db.EmailSubscriptions.AnyAsync(e => e.Email == email))
        {
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<EmailSubscription>> GetAllAsync()
    {
        return await _db.EmailSubscriptions.ToListAsync();
    }
}
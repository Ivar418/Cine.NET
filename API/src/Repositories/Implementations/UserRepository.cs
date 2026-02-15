using Microsoft.EntityFrameworkCore;
using WebApi_PocV1.Domain.Entities;
using WebApi_PocV1.Infrastructure.Database;
using WebApi_PocV1.Repositories.Interfaces;

namespace WebApi_PocV1.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
        => await _db.Users.AsNoTracking().ToListAsync();

    public async Task<User?> GetByIdAsync(int id)
        => await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
}
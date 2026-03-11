using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly ApiDbContext _db;

    public UserRepository(ApiDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
        => await _db.Users.AsNoTracking().ToListAsync();

    public async Task<User?> GetByIdAsync(int id)
        => await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
}
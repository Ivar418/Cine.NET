using API.Domain.Model;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository {
    private readonly ApiDbContext _db;

    public RefreshTokenRepository(ApiDbContext db) {
        _db = db;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token) {
        var result = await _db.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
        return result;
    }

    public async Task AddAsync(RefreshToken refreshToken) {
        await _db.RefreshTokens.AddAsync(refreshToken);
        await SaveChangesAsync();
    }


    public async Task SaveChangesAsync() {
        await _db.SaveChangesAsync();
    }
}
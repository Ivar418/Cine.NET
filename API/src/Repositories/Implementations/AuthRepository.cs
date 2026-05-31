using API.Domain.Common;
using API.Domain.Model;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTOs.Requests;

namespace API.Repositories.Implementations;

public class AuthRepository : IAuthRepository {
    private readonly ApiDbContext _db;

    public AuthRepository(ApiDbContext db) {
        _db = db;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token) {
        var result = await _db.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
        return result;
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken) {
        await _db.RefreshTokens.AddAsync(refreshToken);
        await SaveChangesAsync();
    }


    public async Task SaveChangesAsync() {
        await _db.SaveChangesAsync();
    }

    public async Task<ResultOf<UserCredential?>> AddUserCredentials(UserCredential userCredential) {
        try {
            await _db.AddAsync(userCredential);

            await SaveChangesAsync();
            return ResultOf<UserCredential?>.Success(userCredential);
        }
        catch (Exception ex) {
            return ResultOf<UserCredential?>.Failure(ex.Message);
        }
    }
}
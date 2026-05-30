using API.Domain.Model;
using SharedLibrary.DTOs.Requests;

namespace API.Repositories.Interfaces;

public interface IRefreshTokenRepository {
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task AddAsync(RefreshToken refreshToken);

    Task SaveChangesAsync();
}
using API.Domain.Model;

namespace API.Repositories.Interfaces;

public interface IRefreshTokenRepository {
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task AddAsync(RefreshToken refreshToken);

    Task SaveChangesAsync();
}
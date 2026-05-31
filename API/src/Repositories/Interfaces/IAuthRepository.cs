using API.Domain.Common;
using API.Domain.Model;
using SharedLibrary.DTOs.Requests;

namespace API.Repositories.Interfaces;

public interface IAuthRepository {
    Task<RefreshToken?> GetRefreshTokenAsync(string token);

    Task AddRefreshTokenAsync(RefreshToken refreshToken);

    Task SaveChangesAsync();
    Task<ResultOf<UserCredential?>> AddUserCredentials(UserCredential userCredential);
}
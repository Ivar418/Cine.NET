using API.Domain.Model;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces;

public interface IAuthService {
    public string PasswordHasher(string password);
    public bool VerifyPassword(string password, string passwordHash);
    public Task<AuthResponse?> LoginUser(string userName, string password);
    public Task<AuthResponse?> NewRefreshToken(RefreshRequest request);
}
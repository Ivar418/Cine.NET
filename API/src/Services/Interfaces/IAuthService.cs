using API.Domain.Common;
using API.Domain.Model;
using Microsoft.AspNetCore.Identity.Data;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces;

public interface IAuthService {
    public string PasswordHasher(string password);
    public bool VerifyPassword(string password, string passwordHash);
    public Task<AuthResponse?> LoginUser(string username, string password);
    public Task<AuthResponse?> NewRefreshToken(RefreshRequest request);
    public Task<bool> RevokeRefreshToken(string refreshToken);
    public Task<ResultOf<AuthResponse?>> AddCredentials(User user, string password);
}
using API.Mappers;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class AuthService : IAuthService {
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        IUserService userService,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository) {
        _userService = userService;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public string PasswordHasher(string password) {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash) {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public async Task<AuthResponse?> LoginUser(string userName, string password) {
        var user = await _userService.GetByUserNameAsync(userName);
        if (user.IsFailure || user.Value == null) {
            return null;
        }

        var authUser = await _userService.GetCredentialsByUserId(user.Value.Id);
        if (authUser.IsFailure || authUser.Value == null) return null;
        if (VerifyPassword(password, authUser.Value.PasswordHash) == false) return null;
        var refreshToken = _jwtService.GenerateRefreshToken(user.Value);
        await _refreshTokenRepository.AddAsync(refreshToken);
        return new AuthResponse {
            AccessToken = _jwtService.GenerateAccessToken(user.Value),
            RefreshToken = refreshToken.Token,
            User = UserMapper.ToResponse(user.Value)
        };
    }


    public async Task<AuthResponse?> NewRefreshToken(
        RefreshRequest request) {
        var refreshToken =
            await _refreshTokenRepository
                .GetByTokenAsync(request.RefreshToken);

        if (refreshToken == null)
            return null;

        if (!refreshToken.IsActive())
            return null;

        var user = refreshToken.User;

        var newAccessToken =
            _jwtService.GenerateAccessToken(user);


        refreshToken.Revoke();

        var newRefreshToken =
            _jwtService.GenerateRefreshToken(user);

        await _refreshTokenRepository
            .AddAsync(newRefreshToken);

        await _refreshTokenRepository
            .SaveChangesAsync();

        return new AuthResponse {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            User = UserMapper.ToResponse(user)
        };
    }


    public async Task<bool> RevokeRefreshToken(string refreshToken) {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token != null) {
            token.Revoke();
            await _refreshTokenRepository.SaveChangesAsync();
            return true;
        }
        else {
            return false;
        }
    }
}
using API.Domain.Common;
using API.Domain.Model;
using API.Mappers;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class AuthService : IAuthService {
    private readonly IJwtService _jwtService;
    private readonly IAuthRepository _authRepository;
private readonly IUserRepository _userRE;

    public AuthService(
        IUserRepository userRE,
        IJwtService jwtService,
        IAuthRepository authRepository) {
        _userRE = userRE;
        _jwtService = jwtService;
        _authRepository = authRepository;
    }

    public string PasswordHasher(string password) {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash) {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public async Task<AuthResponse?> LoginUser(string username, string password) {
        var user = await _userRE.GetByUsername(username);
        if (user.IsFailure || user.Value == null) {
            return null;
        }

        var authUser = await _userRE.GetCredentialsByUserId(user.Value.Id);
        if (authUser.IsFailure || authUser.Value == null) return null;
        if (VerifyPassword(password, authUser.Value.PasswordHash) == false) return null;
        var refreshToken = _jwtService.GenerateRefreshToken(user.Value);
        await _authRepository.AddRefreshTokenAsyncWithSave(refreshToken);
        return new AuthResponse {
            AccessToken = _jwtService.GenerateAccessToken(user.Value),
            RefreshToken = refreshToken.Token,
            User = UserMapper.ToResponse(user.Value)
        };
    }


    public async Task<AuthResponse?> NewRefreshToken(
        RefreshRequest request) {
        var refreshToken =
            await _authRepository
                .GetRefreshTokenAsync(request.RefreshToken);

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

        await _authRepository
            .AddRefreshTokenAsyncWithSave(newRefreshToken);

        await _authRepository
            .SaveChangesAsync();

        return new AuthResponse {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            User = UserMapper.ToResponse(user)
        };
    }


    public async Task<bool> RevokeRefreshToken(string refreshToken) {
        var token = await _authRepository.GetRefreshTokenAsync(refreshToken);
        if (token != null) {
            token.Revoke();
            await _authRepository.SaveChangesAsync();
            return true;
        }
        else {
            return false;
        }
    }

    public async Task<ResultOf<AuthResponse?>> AddCredentials(User user, string password) {
        try {
            var passwordHash = PasswordHasher(password);
            var authUser = new UserCredential(userId: user.Id, passwordHash: passwordHash);
            await _authRepository.AddUserCredentials(authUser);

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            _authRepository.AddRefreshTokenAsyncWithSave(refreshToken);
            var authResponse = new AuthResponse {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = UserMapper.ToResponse(user)
            };
            return ResultOf<AuthResponse?>.Success(authResponse);
        }
        catch (Exception e) {
            return ResultOf<AuthResponse?>.Failure(e.Message);
        }
    }
}
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

/// <summary>
/// Implementation of authentication services using BCrypt for hashing and JWT for session management.
/// </summary>
public class AuthService : IAuthService {
    private readonly IJwtService _jwtService;
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IAuthRepository authRepository) {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _authRepository = authRepository;
    }

    /// <summary>
    /// Hashes the password using BCrypt.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if password is null or whitespace.</exception>
    public string PasswordHasher(string password) {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifies the password using BCrypt.
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash) {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    /// <summary>
    /// Authenticates a user and returns a token pair.
    /// </summary>
    public async Task<AuthResponse?> LoginUser(string username, string password) {
        var user = await _userRepository.GetByUsername(username);
        if (user.IsFailure || user.Value == null) {
            return null;
        }

        var authUser = await _userRepository.GetCredentialsByUserId(user.Value.Id);
        if (authUser.IsFailure || authUser.Value == null) return null;
        if (VerifyPassword(password, authUser.Value.PasswordHash) == false) return null;
        var refreshToken = _jwtService.GenerateRefreshToken(user.Value);
        await _authRepository.AddRefreshTokenAsync(refreshToken);
        return new AuthResponse {
            AccessToken = _jwtService.GenerateAccessToken(user.Value),
            RefreshToken = refreshToken.Token,
            User = UserMapper.ToResponse(user.Value)
        };
    }

    /// <summary>
    /// Rotates a refresh token for a new one along with a new access token.
    /// </summary>
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
            .AddRefreshTokenAsync(newRefreshToken);

        return new AuthResponse {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            User = UserMapper.ToResponse(user)
        };
    }

    /// <summary>
    /// Marks a refresh token as revoked in the database.
    /// </summary>
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

    /// <summary>
    /// Creates and saves user credentials (hashed password) and returns initial tokens.
    /// </summary>
    public async Task<ResultOf<AuthResponse?>> AddCredentials(User user, string password) {
        var passwordHash = PasswordHasher(password);
        var authUser = new UserCredential(userId: user.Id, passwordHash: passwordHash);
        if (await _authRepository.AddUserCredentials(authUser) is { IsFailure: true })
            return ResultOf<AuthResponse?>.Failure("Could not add credentials");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken(user);
        await _authRepository.AddRefreshTokenAsync(refreshToken);
        var authResponse = new AuthResponse {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            User = UserMapper.ToResponse(user)
        };
        return ResultOf<AuthResponse?>.Success(authResponse);
    }
}
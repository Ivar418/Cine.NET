using API.Domain.Common;
using API.Domain.Model;
using Microsoft.AspNetCore.Identity.Data;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces;

/// <summary>
/// Defines the core authentication and authorization business logic.
/// </summary>
public interface IAuthService {
    /// <summary>
    /// Hashes a plain-text password using a secure algorithm.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A hashed representation of the password.</returns>
    public string PasswordHasher(string password);

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// </summary>
    /// <param name="password">The plain-text password provided by the user.</param>
    /// <param name="passwordHash">The stored hash to compare against.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    public bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// Validates user credentials and generates a set of JWT tokens.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The plain-text password.</param>
    /// <returns>An <see cref="AuthResponse"/> containing tokens if successful; otherwise, null.</returns>
    public Task<AuthResponse?> LoginUser(string username, string password);

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <returns>A new <see cref="AuthResponse"/> if the token is valid; otherwise, null.</returns>
    public Task<AuthResponse?> NewRefreshToken(RefreshRequest request);

    /// <summary>
    /// Revokes an active refresh token to log a user out.
    /// </summary>
    /// <param name="refreshToken">The token string to revoke.</param>
    /// <returns>True if the token was found and revoked; otherwise, false.</returns>
    public Task<bool> RevokeRefreshToken(string refreshToken);

    /// <summary>
    /// Associates a password with a user entity.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="password">The plain-text password to be hashed and stored.</param>
    /// <returns>A result containing the initial <see cref="AuthResponse"/> if successful.</returns>
    public Task<ResultOf<AuthResponse?>> AddCredentials(User user, string password);
}
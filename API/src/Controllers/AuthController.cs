using System.Security.Claims;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;
using LoginRequest = SharedLibrary.DTOs.Requests.LoginRequest;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

/// <summary>
/// Handles all authentication related requests such as login, registration, and token management.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IUserService userService, ILogger<AuthController> logger) {
        _logger = logger;
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    /// Authenticates a user and returns Access and Refresh tokens.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>200 OK with tokens, or 401 Unauthorized.</returns>
    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
        var loginResponse = await _authService.LoginUser(request.Username, request.Password);
        if (loginResponse == null)
            return Unauthorized("Invalid username or password.");

        return Ok(loginResponse);
    }

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <returns>200 OK with new tokens, or 401 Unauthorized.</returns>
    [Route("refresh")]
    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request) {
        var refreshResponse = await _authService.NewRefreshToken(request);
        if (refreshResponse == null)
            return Unauthorized("Invalid refresh token.");

        return Ok(refreshResponse);
    }

    /// <summary>
    /// Logs out a user by revoking their refresh token.
    /// </summary>
    /// <param name="refreshToken">The token to revoke.</param>
    /// <returns>200 OK if successful, or 400 BadRequest.</returns>
    [Route("logout")]
    [HttpPost]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest refreshToken) {
        var result = await _authService.RevokeRefreshToken(refreshToken.RefreshToken);
        if (!result)
            return BadRequest("Failed to logout.");

        return Ok("Logged out successfully.");
    }

    /// <summary>
    /// Registers a new user and creates their credentials.
    /// </summary>
    /// <param name="request">User creation details.</param>
    /// <returns>200 OK with the created user/token info, or an error status.</returns>
    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request) {
        var result = await _userService.CreateUserAsync(request);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
        return result switch {
            { IsSuccess: true, Value: not null } => Ok(result.Value),
            { IsSuccess: true, Value: null } => StatusCode(500, new { error = "User creation failed" }),
            _ => StatusCode(500, new { error = "Unexpected result" })
        };
    }
}
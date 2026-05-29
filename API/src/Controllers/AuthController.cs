using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) {
        _authService = authService;
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
        var loginResponse = await _authService.LoginUser(request.UserName, request.Password);
        if (loginResponse == null)
            return Unauthorized("Invalid username or password.");

        return Ok(loginResponse);
    }

    [Route("refresh")]
    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request) {
        var refreshResponse = await _authService.NewRefreshToken(request);
        if (refreshResponse == null)
            return Unauthorized("Invalid refresh token.");

        return Ok(refreshResponse);
    }
}
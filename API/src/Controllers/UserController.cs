using Microsoft.AspNetCore.Mvc;
using API.Mappers;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase {
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger) {
        _userService = userService;
        _logger = logger;
    }

    [Authorize]
    [Route("profile/{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUserProfile(int id) {
        var user = await _userService.GetUserByIdAsync(id);
        return user switch {
            { IsFailure: true, Error: "User not found" } => NotFound(new { error = "User not found" }),
            { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
            { IsSuccess: true } => Ok(user.Value),
            _ => StatusCode(500)
        };
    }

    [Authorize]
    [Route("claims")]
    [HttpGet]
    public IActionResult GetClaims() {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}
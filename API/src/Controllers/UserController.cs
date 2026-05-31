using Microsoft.AspNetCore.Mvc;
using API.Mappers;
using API.Services.Interfaces;
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


    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request) {
        try {
            var result = await _userService.CreateUserAsync(request);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });
            return result switch {
                { IsSuccess: true, Value: not null } => Ok(result.Value),
                { IsSuccess: true, Value: null } => StatusCode(500, new { error = "User creation failed" }),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }
}
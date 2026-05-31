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

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        try {
            var users = await _userService.GetAllUsersAsync();

            return users switch {
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(UserMapper.ToResponses(users.Value!)),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }
        catch (Exception) {
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        var response = UserMapper.ToResponse(user.Value!);
        return Ok(response);
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
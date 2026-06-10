using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using API.Mappers;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase {
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger) {
        _userService = userService;
        _logger = logger;
    }

    [Route("me/profile")]
    [HttpGet]
    public async Task<IActionResult> GetUserProfile() {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserByIdAsync(currentUserId);
        return user switch {
            { IsSuccess: true } => Ok(UserMapper.ToResponse(user.Value)),
            { IsFailure: true, Error: "User not found" } => NotFound(new { error = "User not found" }),
            { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
            _ => StatusCode(500)
        };
    }

    [Route("me/favorites")]
    [HttpGet]
    public async Task<IActionResult> GetFavoriteMovies(int id) {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var movies = await _userService.GetFavoriteMoviesAsync(userId: currentUserId);
        return movies switch {
            { IsFailure: true, Error: "User not found" } => NotFound(new { error = "User not found" }),
            { IsSuccess: true } => Ok(movies.Value),
            _ => StatusCode(500, new { error = "An error occurred" })
        };
    }

    [Route("me/favorites/{movieId:int}")]
    [HttpPost]
    public async Task<IActionResult> AddFavoriteMovie(int movieId) {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var movies = await _userService.AddFavoriteMovieAsync(userId: currentUserId, movieId: movieId);
        return movies switch {
            { IsFailure: true, Error: "User not found" } => NotFound(new { error = "User not found" }),
            { IsFailure: true, Error: "Movie not found" } => NotFound(new { error = "Movie not found" }),
            { IsSuccess: true } => Ok(movies.Value),
            _ => StatusCode(500, new { error = "An error occurred" })
        };
    }

    [Route("me/favorites/{movieId:int}")]
    [HttpDelete]
    public async Task<IActionResult> RemoveFavoriteMovie(int movieId) {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var movies = await _userService.RemoveFavoriteMovieAsync(userId: currentUserId, movieId: movieId);
        return movies switch {
            { IsFailure: true, Error: "User not found" } => NotFound(new { error = "User not found" }),
            { IsFailure: true, Error: "Movie not found" } => NotFound(new { error = "Movie not found" }),
            { IsSuccess: true } => Ok(movies.Value),
            _ => StatusCode(500, new { error = "An error occurred" })
        };
    }
}
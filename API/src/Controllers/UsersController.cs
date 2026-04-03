using Microsoft.AspNetCore.Mvc;
using API.Mappers;
using API.Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing all users when successful,
        /// or an error response when retrieval fails.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try {
            var users = await _userService.GetAllUsersAsync();
            
            return users switch
            {
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(UserMapper.ToResponses(users.Value!)),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred" });
            }
            
        }

        /// <summary>
        /// Retrieves a single user by its identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user when found,
        /// or <c>404 Not Found</c> when the user does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            var response = UserMapper.ToResponse(user.Value!);
            return Ok(response);
        }
    }
}

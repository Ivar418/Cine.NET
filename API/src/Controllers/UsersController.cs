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

        // ORM: async read via service → repository → EF Core
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsersAsync();

            if (result.IsFailure)
                return BadRequest(result.Error);

            var response = UserMapper.ToResponses(result.Value!);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (result.IsFailure)
                return NotFound(result.Error);

            var response = UserMapper.ToResponse(result.Value!);
            return Ok(response);
        }
    }
}

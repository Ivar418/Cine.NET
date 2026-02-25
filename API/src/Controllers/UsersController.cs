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
            var users = await _userService.GetAllUsersAsync();
            var response = UserMapper.ToResponses(users);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            var response = UserMapper.ToResponse(user);
            return Ok(response);
        }
    }
}

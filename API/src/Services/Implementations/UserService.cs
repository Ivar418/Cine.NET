using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultOf<IReadOnlyList<User>>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();
            return ResultOf<IReadOnlyList<User>>.Success(users);
        }

        public async Task<ResultOf<User>> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return ResultOf<User>.Failure($"User with id {id} not found.");
            
            return ResultOf<User>.Success(user);
        }
    }
}
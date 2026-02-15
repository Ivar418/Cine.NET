using WebApi_PocV1.Domain.Entities;
using WebApi_PocV1.Repositories.Interfaces;
using WebApi_PocV1.Services.Interfaces;

namespace WebApi_PocV1.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<User>> GetAllUsersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}

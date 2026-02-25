using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
    }
}

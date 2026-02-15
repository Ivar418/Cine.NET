using WebApi_PocV1.Domain.Entities;

namespace WebApi_PocV1.Services.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
    }
}

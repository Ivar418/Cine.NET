using WebApi_PocV1.Domain.Entities;

namespace WebApi_PocV1.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
    }
}
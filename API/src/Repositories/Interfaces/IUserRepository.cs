using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
    }
}
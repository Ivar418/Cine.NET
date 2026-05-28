using API.Domain.Common;
using API.Domain.Model;
using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces {
    public interface IUserRepository {
        Task<ResultOf<IReadOnlyList<User>>> GetAllAsync();
        Task<ResultOf<User?>> GetByIdAsync(int id);
        Task<ResultOf<User?>> GetByUserNameAsync(string userName);
        Task<ResultOf<UserCredential?>> GetCredentialsByUserId(int id);
    }
}
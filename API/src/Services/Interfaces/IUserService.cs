using API.Domain.Common;
using API.Domain.Model;
using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces {
    public interface IUserService {
        Task<ResultOf<IReadOnlyList<User>>> GetAllUsersAsync();
        Task<ResultOf<User?>> GetUserByIdAsync(int id);
        Task<ResultOf<User?>> GetByUserNameAsync(string userName);
        Task<ResultOf<UserCredential?>> GetCredentialsByUserId(int id);
    }
}
using API.Domain.Common;
using API.Domain.Model;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;

namespace API.Repositories.Interfaces {
    public interface IUserRepository {
        Task<ResultOf<IReadOnlyList<User>>> GetAllAsync();
        Task<ResultOf<User?>> GetByIdAsync(int id);
        Task<ResultOf<User?>> GetByUsername(string username);
        Task<ResultOf<UserCredential?>> GetCredentialsByUserId(int id);
        Task<ResultOf<User?>> AddUserAsync(CreateUserRequest user);
    }
}
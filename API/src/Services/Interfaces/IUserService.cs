using API.Domain.Common;
using API.Domain.Model;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces {
    public interface IUserService {
        Task<ResultOf<IReadOnlyList<User>>> GetAllUsersAsync();
        Task<ResultOf<User?>> GetUserByIdAsync(int id);
        Task<ResultOf<User?>> GetByUsername(string username);
        Task<ResultOf<UserCredential?>> GetCredentialsByUserId(int id);
        Task<ResultOf<AuthResponse?>> CreateUserAsync(CreateUserRequest user);
    }
}
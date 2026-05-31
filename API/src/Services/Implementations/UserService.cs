using API.Domain.Common;
using API.Domain.Model;
using SharedLibrary.Domain.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations {
    public class UserService : IUserService {
        private readonly IUserRepository _repository;
        private readonly IAuthService _authService;

        public UserService(IUserRepository repository, IAuthService authService) {
            _repository = repository;
            _authService = authService;
        }

        public async Task<ResultOf<IReadOnlyList<User>>> GetAllUsersAsync() {
            return await _repository.GetAllAsync();
        }

        public async Task<ResultOf<User?>> GetUserByIdAsync(int id) {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ResultOf<User?>> GetByUsername(string username) {
            return await _repository.GetByUsername(username);
        }

        public async Task<ResultOf<UserCredential?>> GetCredentialsByUserId(int id) {
            return await _repository.GetCredentialsByUserId(id);
        }

        public async Task<ResultOf<AuthResponse?>> CreateUserAsync(CreateUserRequest user) {
            if (await _repository.GetByUsername(user.UserName) is { IsSuccess: true, Value: not null }) {
                return ResultOf<AuthResponse?>.Failure("Username already exists");
            }

            var addUserAsync = await _repository.AddUserAsync(user);
            if (addUserAsync is { IsFailure: true } || addUserAsync.Value is null) {
                return ResultOf<AuthResponse?>.Failure(addUserAsync.Error ?? "User creation failed");
            }
            await _repository.SaveChangesAsync();
            var addCredentialsAsync = await _authService.AddCredentials(addUserAsync.Value, user.Password);
            if (addCredentialsAsync.IsFailure) {
                return ResultOf<AuthResponse?>.Failure(addCredentialsAsync.Error ??
                                                       "Failed to create user credentials");
            }

            await _repository.SaveChangesAsync();
            return ResultOf<AuthResponse?>.Success(addCredentialsAsync.Value);
        }
    }
}
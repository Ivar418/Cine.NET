using API.Domain.Common;
using API.Domain.Model;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.Users;

namespace API.Services.Interfaces {
    public interface IUserService {
        Task<ResultOf<IReadOnlyList<User>>> GetAllUsersAsync();
        Task<ResultOf<User>> GetUserByIdAsync(int id);
        Task<ResultOf<User?>> GetByUsername(string username);
        Task<ResultOf<UserCredential?>> GetCredentialsByUserId(int id);
        Task<ResultOf<AuthResponse?>> CreateUserAsync(CreateUserRequest user);
        Task<ResultOf<UserFavoriteMoviesListResponse>> GetFavoriteMoviesAsync(int userId);
        Task<ResultOf<UserFavoriteMoviesListResponse>> AddFavoriteMovieAsync(int userId, int movieId);
        Task<ResultOf<UserFavoriteMoviesListResponse>> RemoveFavoriteMovieAsync(int userId, int movieId);
    }
}
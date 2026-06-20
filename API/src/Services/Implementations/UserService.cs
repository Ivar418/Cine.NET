using API.Domain.Common;
using API.Domain.Model;
using API.Mappers;
using SharedLibrary.Domain.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.Users;

namespace API.Services.Implementations;

/// <summary>
/// Implementation of user management business logic.
/// </summary>
public class UserService : IUserService {
    private readonly IUserRepository _repository;
    private readonly IAuthService _authService;
    private readonly IMovieRepository _movieRepository;

    public UserService(IUserRepository repository, IAuthService authService, IMovieRepository movieRepository) {
        _repository = repository;
        _authService = authService;
        _movieRepository = movieRepository;
    }

    public async Task<ResultOf<IReadOnlyList<User>>> GetAllUsersAsync() {
        return await _repository.GetAllAsync();
    }

    public async Task<ResultOf<User>> GetUserByIdAsync(int id) {
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

    public async Task<ResultOf<User>> UpdateUserAsync(int id, UpdateUserRequest request) {
        var userResult = await _repository.GetByIdAsync(id);
        if (userResult.IsFailure) {
            return ResultOf<User>.Failure(userResult.Error ?? "User not found");
        }

        var user = userResult.Value!;

        if (!string.IsNullOrEmpty(request.FirstName)) {
            user.ChangeName(firstName: request.FirstName);
        }

        if (!string.IsNullOrEmpty(request.LastName)) {
            user.ChangeName(lastName: request.LastName);
        }

        if (!string.IsNullOrEmpty(request.Email)) {
            user.ChangeEmail(request.Email);
        }

        if (!string.IsNullOrEmpty(request.Password)) {
            var updatePasswordResult = await _authService.UpdatePassword(user, request.Password);
            if (updatePasswordResult.IsFailure) {
                return ResultOf<User>.Failure(updatePasswordResult.Error ?? "Failed to update password");
            }
        }

        await _repository.SaveChangesAsync();
        return ResultOf<User>.Success(user);
    }

    public async Task<ResultOf<UserFavoriteMoviesListResponse>> GetFavoriteMoviesAsync(int userId) {
        return await GetUserByIdAsync(userId) switch {
            { IsSuccess: true, Value: var user } => ResultOf<UserFavoriteMoviesListResponse>.Success(
                FavoriteMappers.ToFavoriteListResponse(user!)),
            { IsFailure: true, Error: "User not found" } => ResultOf<UserFavoriteMoviesListResponse>.Failure(
                "User not found"),
            _ => ResultOf<UserFavoriteMoviesListResponse>.Failure("An error occurred")
        };
    }

    public async Task<ResultOf<UserFavoriteMoviesListResponse>> AddFavoriteMovieAsync(int userId, int movieId) {
        var userResult = await GetUserByIdAsync(userId);
        var movie = await _movieRepository.GetMovieByIdAsync(movieId);
        if (movie is { IsFailure: true }) {
            return ResultOf<UserFavoriteMoviesListResponse>.Failure("Movie not found");
        }

        if (userResult.IsFailure)
            return ResultOf<UserFavoriteMoviesListResponse>.Failure(userResult.Error ?? "User not found");

        var user = userResult.Value!;

        user.AddFavoriteMovie(movieId);

        await _repository.SaveChangesAsync();

        return ResultOf<UserFavoriteMoviesListResponse>.Success(
            FavoriteMappers.ToFavoriteListResponse(user)
        );
    }

    public async Task<ResultOf<UserFavoriteMoviesListResponse>> RemoveFavoriteMovieAsync(int userId, int movieId) {
        var userResult = await GetUserByIdAsync(userId);
        if (await _movieRepository.GetMovieByIdAsync(movieId) is { IsFailure: true }) {
            return ResultOf<UserFavoriteMoviesListResponse>.Failure("Movie not found");
        }

        if (userResult.IsFailure)
            return ResultOf<UserFavoriteMoviesListResponse>.Failure(userResult.Error);

        var user = userResult.Value!;

        user.RemoveFavoriteMovie(movieId);

        await _repository.SaveChangesAsync();

        return ResultOf<UserFavoriteMoviesListResponse>.Success(
            FavoriteMappers.ToFavoriteListResponse(user)
        );
    }
}
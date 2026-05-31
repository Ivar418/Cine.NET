using API.Domain.Common;
using API.Domain.Model;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using SharedLibrary.DTOs.Requests;

namespace API.Repositories.Implementations;

public class UserRepository : IUserRepository {
    private readonly ApiDbContext _db;

    public UserRepository(ApiDbContext db) {
        _db = db;
    }

    public async Task<ResultOf<IReadOnlyList<User>>> GetAllAsync() {
        try {
            var users = await _db.Users
                .AsNoTracking()
                .ToListAsync();

            return ResultOf<IReadOnlyList<User>>.Success(users);
        }
        catch (Exception ex) {
            return ResultOf<IReadOnlyList<User>>.Failure(ex.Message);
        }
    }

    public async Task<ResultOf<User?>> GetByIdAsync(int id) {
        try {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return ResultOf<User?>.Failure("User not found");

            return ResultOf<User?>.Success(user);
        }
        catch (Exception ex) {
            return ResultOf<User?>.Failure(ex.Message);
        }
    }

    public async Task<ResultOf<User?>> GetByUsername(string username) {
        try {
            var user = await _db.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
            if (user == null) return ResultOf<User?>.Failure("User not found");
            return ResultOf<User?>.Success(user);
        }
        catch (Exception ex) {
            return ResultOf<User?>.Failure(ex.Message);
        }
    }

    public async Task<ResultOf<UserCredential?>> GetCredentialsByUserId(int id) {
        try {
            var credentials = await _db.UserCredentials.Where(c => c.UserId == id).FirstOrDefaultAsync();
            return credentials == null
                ? ResultOf<UserCredential?>.Failure("Credentials not found")
                : ResultOf<UserCredential?>.Success(credentials);
        }
        catch (Exception ex) {
            return ResultOf<UserCredential?>.Failure(ex.Message);
        }
    }

    public async Task<ResultOf<User>> AddUserAsync(CreateUserRequest user) {
        var userExists = await _db.Users.AnyAsync(u => u.UserName == user.UserName);
        if (userExists) return ResultOf<User>.Failure("User already exists");
        var addedUser = await _db.AddAsync(new User(
            userName: user.UserName,
            firstName: user.FirstName,
            lastName: user.LastName,
            email: user.Email
        ));
        return ResultOf<User>.Success(addedUser.Entity);
    }

    public async Task SaveChangesAsync() {
        await _db.SaveChangesAsync();
    }
}
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

public class AuthService : IAuthService {
    public string PasswordHasher(string password) {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash) {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public Task<User?> LoginUser(string userName, string password) {
        throw new NotImplementedException();
    }
}
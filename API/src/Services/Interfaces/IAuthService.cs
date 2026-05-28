using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface IAuthService {
    public string PasswordHasher(string password);
    public bool VerifyPassword(string password, string passwordHash);
    public Task<User?> LoginUser(string userName, string password);
}
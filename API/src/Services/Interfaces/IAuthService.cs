using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface IAuthService {
    public string PasswordHasher(string password);
    public bool VerifyPassword(string password, string passwordHash);
    public void LoginUser(string userName, string password);
    public void refreshAuth(string refreshToken);
    public void generateAccessKey(User user);
}
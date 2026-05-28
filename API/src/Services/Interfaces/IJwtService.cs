using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface IJwtService {
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    bool ValidateAccessToken(string token);
}
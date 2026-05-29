using API.Domain.Model;
using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface IJwtService {
    string GenerateAccessToken(User user);

    RefreshToken GenerateRefreshToken(User user);

    bool ValidateAccessToken(string token);
}
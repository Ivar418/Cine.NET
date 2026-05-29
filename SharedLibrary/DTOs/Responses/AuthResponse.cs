using SharedLibrary.Domain.Entities;

namespace SharedLibrary.DTOs.Responses;

public class AuthResponse {
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required User User { get; set; }
}
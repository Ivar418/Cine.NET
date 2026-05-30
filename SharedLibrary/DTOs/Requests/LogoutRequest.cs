namespace SharedLibrary.DTOs.Requests;

public class LogoutRequest {
    public required string RefreshToken { get; set; }
}
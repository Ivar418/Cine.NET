namespace SharedLibrary.DTOs.Requests;

public class RefreshRequest {
    public required string RefreshToken { get; set; }
    public required int UserId { get; set; }
}
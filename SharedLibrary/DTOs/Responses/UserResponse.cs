namespace SharedLibrary.DTOs.Responses;

public class UserResponse {
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? PhotoId { get; set; }
    public string? PhotoUrl { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
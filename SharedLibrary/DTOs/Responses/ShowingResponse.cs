namespace SharedLibrary.DTOs.Responses;

public class ShowingResponse
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int AuditoriumId { get; set; }
    public bool Is3D { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public string AuditoriumLayoutSnapshot { get; set; } = "[]";
}
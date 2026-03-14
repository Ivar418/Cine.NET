namespace SharedLibrary.DTOs.Responses;

// Showing response that includes MovieTitle and AuditoriumName
public class ShowingDisplayResponse
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int AuditoriumId { get; set; }
    public string MovieTitle { get; set; }
    public string AuditoriumName { get; set; }
    public bool Is3D { get; set; }
    public DateTimeOffset StartsAt { get; set; }
}
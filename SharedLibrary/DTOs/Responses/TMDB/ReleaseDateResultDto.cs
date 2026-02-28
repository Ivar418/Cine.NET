namespace SharedLibrary.DTOs.Responses.TMDB;

public class ReleaseDateResultDto
{
    public string Iso_3166_1 { get; set; } // Country code
    public List<ReleaseDateDto> ReleaseDates { get; set; }
}
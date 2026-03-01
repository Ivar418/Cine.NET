namespace SharedLibrary.DTOs.Responses.TMDB;

public class ReleaseInformationPerCountryDto
{
    public string Iso_3166_1 { get; set; } // Country code
    public List<ReleaseInformationDto> ReleaseDates { get; set; }
}
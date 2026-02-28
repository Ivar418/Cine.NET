namespace SharedLibrary.DTOs.Responses.TMDB;

public class MovieReleaseDatesDto
{
    public int Id { get; set; } // TMDB movie ID
    public List<ReleaseInformationPerCountryDto> Results { get; set; }
}
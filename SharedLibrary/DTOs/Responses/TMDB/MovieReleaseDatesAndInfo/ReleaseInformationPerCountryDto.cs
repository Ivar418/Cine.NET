namespace SharedLibrary.DTOs.Responses.TMDB.MovieReleaseDatesAndInfo;

public class ReleaseInformationPerCountryDto
{
    public string iso_3166_1 { get; set; } // Country code
    public List<ReleaseInformationDto> release_dates { get; set; }
}
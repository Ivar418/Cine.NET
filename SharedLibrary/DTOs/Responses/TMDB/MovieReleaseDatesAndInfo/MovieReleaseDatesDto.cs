using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB.MovieReleaseDatesAndInfo;

public class MovieReleaseDatesDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; } // TMDB movie ID
    [JsonPropertyName("results")]
    public List<ReleaseInformationPerCountryDto> Results { get; set; }
}
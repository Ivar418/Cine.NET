using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB;

public class MovieSearchResultListDto
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<MovieSearchItemDto> Results { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
}
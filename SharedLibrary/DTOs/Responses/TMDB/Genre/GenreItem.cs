using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB.Genre;

public class GenreItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB;

public class GenreDto
{
    [JsonPropertyName("id")]

    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
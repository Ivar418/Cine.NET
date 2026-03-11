using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB;

public class BelongsToCollectionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; }
    [JsonPropertyName("backdrop_path")]
    public string BackdropPath { get; set; }
}
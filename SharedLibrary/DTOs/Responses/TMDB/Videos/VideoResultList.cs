using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB.Videos;

public class VideoResultList
{
    [JsonPropertyName("id")] public int Id { get; set; } = 0;
    [JsonPropertyName("results")] public List<VideoResultItem> Results { get; set; } = new List<VideoResultItem>();
}
using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB.Videos;

public class VideoResultItem
{
    [JsonPropertyName("iso_639_1")] public string Iso6391 { get; set; } = string.Empty;
    [JsonPropertyName("iso_3166_1")] public string Iso31661 { get; set; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("key")] public string Key { get; set; } = string.Empty;
    [JsonPropertyName("site")] public string Site { get; set; } = string.Empty;
    [JsonPropertyName("size")] public int Size { get; set; } = 1080;
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("official")] public bool Official { get; set; } = false;
    [JsonPropertyName("published_at")] public DateTimeOffset  PublishedAt { get; set; }
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
}
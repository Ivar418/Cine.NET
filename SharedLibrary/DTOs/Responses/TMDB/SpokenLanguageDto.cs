using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB;

public class SpokenLanguageDto
{
    [JsonPropertyName("english_name")]
    public string EnglishName { get; set; }
    [JsonPropertyName("iso_639_1")]
    public string Iso_639_1 { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
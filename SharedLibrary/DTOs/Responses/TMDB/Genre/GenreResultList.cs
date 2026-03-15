using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB.Genre;

public class GenreResultList
{
    [JsonPropertyName("genres")] public List<GenreItem> Genres { get; set; }
}
namespace SharedLibrary.DTOs.Responses;

public class GenreResponse
{
    public int TmdbId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}
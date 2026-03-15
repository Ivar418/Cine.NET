namespace SharedLibrary.DTOs.Responses;

public class MovieResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int TmdbId { get; init; }
    public string? Language { get; init; }
    public string? PosterPath { get; init; }
    public int? Runtime { get; init; }
    public string? ImdbId { get; init; }
    public string? ReleaseDate { get; init; }
    public string? About { get; init; }
    public string? AgeIndication { get; init; }
    public string? SpokenLanguageName { get; init; }
    public string? SpokenLanguageCodeIso6391 { get; init; }
    public List<int>? GenresIds { get; init; }
    public string RowCreatedTimestampUtc { get; init; } = string.Empty;
    public string? RowUpdatedTimestampUtc { get; init; } 
}
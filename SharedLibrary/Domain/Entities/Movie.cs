namespace SharedLibrary.Domain.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TmdbId { get; set; } = 0;
    public string? Language { get; set; }
    public string? PosterUrl { get; set; }
    public int? Runtime { get; set; }
    public string? Auditorium { get; set; }
    public string? ImdbId { get; set; }
    public string? ReleaseDate { get; set; }
    public string? About { get; set; }
    public string? AgeIndication { get; set; }
    public string? SpokenLanguageName { get; set; }
    public string? SpokenLanguageCodeIso6391 { get; set; }
    public string? Genres { get; set; }
}
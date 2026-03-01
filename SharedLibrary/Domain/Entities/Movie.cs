using System.ComponentModel.DataAnnotations.Schema;
using SharedLibrary.DTOs.Responses.TMDB;

namespace SharedLibrary.Domain.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TmdbId { get; set; } = 0;
    public string? Language { get; set; }
    public string? PosterUrl { get; set; }
    public int? Runtime { get; set; }
    public string? ImdbId { get; set; }
    public string? ReleaseDate { get; set; }
    public string? About { get; set; }
    public string? AgeIndication { get; set; }
    public string? SpokenLanguageName { get; set; }
    public string? SpokenLanguageCodeIso6391 { get; set; }
    public List<int>? GenresIds { get; set; }

    [Column("row_created_timestamp_utc")] public string RowCreatedTimestampUtc { get; init; }
    [Column("row_updated_timestamp_utc")] public string? RowUpdatedTimestampUtc { get; set; }
    [Column("row_deleted_timestamp_utc")] public string? RowDeletedTimestampUtc { get; set; }
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SharedLibrary.DTOs.Responses.TMDB;

namespace SharedLibrary.Domain.Entities;

public class Movie
{
    [Column("id")] public int Id { get; set; }

    [Column("title")] public string Title { get; set; } = string.Empty;

    [Column("tmdb_id")] public int TmdbId { get; set; } = 0;

    //Defines the language of the information about the movie, not the language of the movie itself. E.g. "en" for English, "nl" for Dutch.
    [Column("information_language")]
    public string InformationLanguage { get; set; } =
        "und"; // "und" stands for undefined, used when language is not specified or unknown.

    [Column("language")] public string? Language { get; set; }

    [Column("poster_url")] public string? PosterPath { get; set; }
    [Column("backdrop_url")] public string? BackdropPath { get; set; }
    [Column("youtube_trailer_key")] public string? YoutubeTrailerKey { get; set; }
    [Column("runtime")] public int? Runtime { get; set; }

    [Column("imdb_id")] public string? ImdbId { get; set; }

    [Column("release_date")] public string? ReleaseDate { get; set; }

    [Column("about")] public string? About { get; set; }

    [Column("age_indication")] public string? AgeIndication { get; set; }

    [Column("spoken_language_name")] public string? SpokenLanguageName { get; set; }

    [Column("spoken_language_code_iso6391")]
    public string? SpokenLanguageCodeIso6391 { get; set; }

    [Column("genres_ids")] public List<int>? GenresIds { get; set; }


    [Column("row_created_timestamp_utc")] public DateTimeOffset RowCreatedTimestampUtc { get; init; }

    [Column("row_updated_timestamp_utc")] public DateTimeOffset? RowUpdatedTimestampUtc { get; set; }

    [Column("row_deleted_timestamp_utc")] public DateTimeOffset? RowDeletedTimestampUtc { get; init; }

    // --- Helper factory method to generate current UTC string ---
    public static string CurrentUtcTimestamp() => DateTimeOffset.UtcNow.ToString("O");
}
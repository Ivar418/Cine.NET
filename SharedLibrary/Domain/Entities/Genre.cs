using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Domain.Entities;

public class Genre
{
    [Column("id")] public int Id { get; set; }
    [Column("tmdb_id")] public int TmdbId { get; set; }
    [Column("name")] public string Name { get; set; } = string.Empty;

    [Column("language")]
    public string Language { get; set; } =
        "und"; // "und" stands for undefined, used when language is not specified or unknown.
}
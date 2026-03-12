using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB.MovieReleaseDatesAndInfo
{
    public class ReleaseInformationDto
    {
        [JsonPropertyName( "certification")]
        public string Certification { get; set; } // e.g., "PG-13", "12", "M/12"
        [JsonPropertyName( "descriptors")]
        public List<string> Descriptors { get; set; } // usually empty
        [JsonPropertyName( "iso_3166_1")]
        public string Iso_639_1 { get; set; } // language code, often empty
        [JsonPropertyName( "release_notes")]
        public string Note { get; set; } // optional note about the release
        [JsonPropertyName( "release_date")]
        public DateTime ReleaseDate { get; set; } // release date
        [JsonPropertyName( "type")]
        public int Type { get; set; } // type of release
    }
}
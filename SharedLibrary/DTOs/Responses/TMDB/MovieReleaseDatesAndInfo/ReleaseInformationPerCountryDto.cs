using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Responses.TMDB.MovieReleaseDatesAndInfo;

/// <summary>
/// Represents release information for a movie within a specific country, including
/// the country code and a list of release details for that country.
/// </summary>
public class ReleaseInformationPerCountryDto
{
    /// <summary>
    /// Gets or sets the ISO 3166-1 alpha-2 country code corresponding to the country
    /// where the release applies. This property represents the region-specific
    /// release location for a movie or content.
    /// </summary>
    [JsonPropertyName( "iso_3166_1")]
    public string CountryOfRelease { get; set; } // Country code

    /// <summary>
    /// A collection of release information per country for a specific movie.
    /// Each entry in the list contains details about the release dates and related information
    /// for a particular country.
    /// </summary>
    public List<ReleaseInformationDto> release_dates { get; set; }
}
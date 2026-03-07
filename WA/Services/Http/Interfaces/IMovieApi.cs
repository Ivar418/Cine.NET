using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.TMDB;

namespace WA.Services.Http.Interfaces;

public interface IMovieApiClient
{
    Task<List<MovieResponse>?> GetAllMoviesAsync();
    Task<bool> DeleteMovieAsync(int id);

    /// <summary>
    /// Searches TMDB for movies matching the query string.
    /// </summary>
    Task<MovieSearchResultListDto?> SearchTmdbAsync(
        string query,
        string language = "nl",
        bool includeAdult = false,
        int? page = null,
        string? primaryReleaseYear = null);

    /// <summary>
    /// Adds a movie to the system by its TMDB ID.
    /// Returns the created MovieResponse on success, null on failure.
    /// </summary>
    Task<(bool Success, string? ErrorMessage, MovieResponse? Movie)> AddMovieFromTmdbAsync(
        int tmdbId,
        string language = "nl");
}

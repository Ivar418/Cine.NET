using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.TMDB;
using SharedLibrary.DTOs.Responses.TMDB.Genre;

namespace WA.Services.Http.Interfaces;

public interface IMovieApiClient
{
    Task<List<MovieResponse>?> GetAllMoviesAsync();
    
    Task<MovieResponse?> GetMovieByIdAsync(int id);
    
    Task<bool> DeleteMovieAsync(int tmdbId);

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
    Task<(bool Success, string? ErrorMessage, MovieResponse? Movie)> AddMovieFromTmdbAsync(int tmdbId);
    
    Task<GenreResponse?> GetGenreByIdAsync(int tmdbGenreId, string language = "nl");
}

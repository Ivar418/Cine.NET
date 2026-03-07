using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.TMDB;
using WA.Services.Http.Interfaces;

namespace WA.Services.Http;

public class MovieApiClient : IMovieApiClient
{
    private readonly HttpClient _http;
    private const string BasePath = "api/movies";

    public MovieApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<MovieResponse>?> GetAllMoviesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<MovieResponse>>(BasePath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[MovieApiClient] GetAllMovies failed: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"{BasePath}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[MovieApiClient] DeleteMovie({id}) failed: {ex.Message}");
            return false;
        }
    }

    public async Task<MovieSearchResultListDto?> SearchTmdbAsync(
        string query,
        string language = "nl",
        bool includeAdult = false,
        int? page = null,
        string? primaryReleaseYear = null)
    {
        try
        {
            var url = $"{BasePath}/tmdb/search?query={Uri.EscapeDataString(query)}" +
                      $"&include_adult={includeAdult}" +
                      $"&language={language}";

            if (page.HasValue)
                url += $"&page={page.Value}";

            if (!string.IsNullOrEmpty(primaryReleaseYear))
                url += $"&primary_release_year={primaryReleaseYear}";

            return await _http.GetFromJsonAsync<MovieSearchResultListDto>(url);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[MovieApiClient] SearchTmdb failed: {ex.Message}");
            return null;
        }
    }

    public async Task<(bool Success, string? ErrorMessage, MovieResponse? Movie)> AddMovieFromTmdbAsync(
        int tmdbId,
        string language = "nl")
    {
        try
        {
            var response = await _http.PostAsync(
                $"{BasePath}?tmdbId={tmdbId}&language={language}",
                content: null);

            if (response.IsSuccessStatusCode)
            {
                var movie = await response.Content.ReadFromJsonAsync<MovieResponse>();
                return (true, null, movie);
            }

            var errorMessage = response.StatusCode switch
            {
                HttpStatusCode.Conflict  => "This movie is already in the system.",
                HttpStatusCode.NotFound  => "Movie not found on TMDB.",
                HttpStatusCode.BadRequest => "Invalid TMDB ID.",
                _ => "An unexpected error occurred. Please try again."
            };

            return (false, errorMessage, null);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[MovieApiClient] AddMovieFromTmdb({tmdbId}) failed: {ex.Message}");
            return (false, "An unexpected error occurred. Please try again.", null);
        }
    }
}

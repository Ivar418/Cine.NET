using System.Net.Http.Json;
using SharedLibrary.DTOs.Responses;
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
}
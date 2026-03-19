using System.Net.Http.Json;
using SharedLibrary.DTOs.Responses;
using WA.Services.Http.Interfaces;
using 

namespace WA.Services.Http;

public class ShowingApi : IShowingApi
{
    private readonly HttpClient _http;
    private const string BasePath = "api/showings";

    public ShowingApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ShowingsWithPricesResponse>> GetShowingsWithPricesAsync()
    {
        var result = await _http.GetFromJsonAsync<List<ShowingsWithPricesResponse>>(
            $"{BasePath}/with-prices"
        );

        return result ?? [];
    }

    public async Task<ShowingResponse?> GetShowingByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<ShowingResponse>($"{BasePath}/{id}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ShowingApiClient] GetShowingById({id}) failed: {ex.Message}");
            return null;
        }
    }

    public async Task<ShowingDisplayResponse?> GetShowingDisplayByIdAsync(int id)
    {
        try
        {
            var response = await _http.GetAsync($"{BasePath}/{id}/details");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShowingDisplayResponse>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ShowingApiClient] GetShowingDisplayById({id}) failed: {ex.Message}");
            return null;
        }
    }

    public async Task<ShowingsWithPricesResponse?> GetShowingPricesAsync(int showingId)
    {
        var result = await _http.GetFromJsonAsync<ShowingsWithPricesResponse>(
            $"api/showings/{showingId}/prices");

        return result?.Value;
    }
    
    public async Task<IReadOnlyList<ShowingResponse>> GetUpcomingShowingsByMovieIdAsync(int movieId)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<List<ShowingResponse>>(
                $"{BasePath}/movie/{movieId}/upcoming"
            );
            return result ?? [];
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ShowingApi] GetUpcomingShowingsByMovieId({movieId}) failed: {ex.Message}");
            return [];
        }
    }
}

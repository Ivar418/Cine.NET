using System.Net.Http.Json;
using SharedLibrary.DTOs.Responses;
using WA.Services.Http.Interfaces;

namespace WA.Services.Http;

public class ShowingApi : IShowingApi
{
    private readonly HttpClient _http;

    public ShowingApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ShowingsWithPricesResponse>> GetShowingsWithPricesAsync()
    {
        var result = await _http.GetFromJsonAsync<List<ShowingsWithPricesResponse>>(
            "api/showings/with-prices"
        );

        return result ?? [];
    }

    public async Task<ShowingsWithPricesResponse?> GetShowingPricesAsync(int id)
    {
        return await _http.GetFromJsonAsync<ShowingsWithPricesResponse>(
            $"api/showings/{id}/with-prices"
        );
    }
}
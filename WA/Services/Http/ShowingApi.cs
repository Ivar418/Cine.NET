using System.Net.Http.Json;
using SharedLibrary.DTOs.Responses;
using WA.Services.Http.Interfaces;

namespace WA.Services.Http;

/// <summary>
/// HTTP client for interacting with Showing-related API endpoints.
/// </summary>
public class ShowingApi : IShowingApi
{
    private readonly HttpClient _http;
    private const string BasePath = "api/showings";

    /// <summary>
    /// Initializes a new instance of the ShowingApi.
    /// </summary>
    /// <param name="http">The configured HttpClient.</param>
    public ShowingApi(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Retrieves all Showings including pricing information.
    /// </summary>
    /// <returns>
    /// A read-only list of <see cref="ShowingsWithPricesResponse"/>.
    /// Returns an empty list if no data is found.
    /// </returns>
    public async Task<IReadOnlyList<ShowingsWithPricesResponse>> GetShowingsWithPricesAsync()
    {
        var result = await _http.GetFromJsonAsync<List<ShowingsWithPricesResponse>>(
            $"{BasePath}/with-prices"
        );

        return result ?? [];
    }

    /// <summary>
    /// Retrieves a Showing by its ID.
    /// </summary>
    /// <param name="id">The Showing ID.</param>
    /// <returns>
    /// A <see cref="ShowingResponse"/> if found; otherwise null.
    /// </returns>
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

    /// <summary>
    /// Retrieves detailed display information for a Showing by ID.
    /// </summary>
    /// <param name="id">The Showing ID.</param>
    /// <returns>
    /// A <see cref="ShowingDisplayResponse"/> if found; otherwise null.
    /// </returns>
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
    
    /// <summary>
    /// Retrieves all Showing display data, optionally filtered by date.
    /// </summary>
    /// <param name="date">Optional date filter.</param>
    /// <returns>
    /// A read-only list of <see cref="ShowingDisplayResponse"/>.
    /// Returns an empty list if no data is found.
    /// </returns>
    public async Task<IReadOnlyList<ShowingDisplayResponse>> GetShowingDisplayAsync(DateOnly? date = null)
    {
        var url = $"{BasePath}/details";

        if (date is not null)
        {
            url += $"?date={date:yyyy-MM-dd}";
        }

        var result = await _http.GetFromJsonAsync<List<ShowingDisplayResponse>>(url);

        return result ?? [];
    }

    /// <summary>
    /// Retrieves pricing information for a specific Showing.
    /// </summary>
    /// <param name="showingId">The Showing ID.</param>
    /// <returns>
    /// A <see cref="ShowingsWithPricesResponse"/> if found; otherwise null.
    /// </returns>
    public async Task<ShowingsWithPricesResponse?> GetShowingPricesAsync(int showingId)
    {
        var result = await _http.GetFromJsonAsync<ShowingsWithPricesResponse>(
            $"api/showings/{showingId}/prices");

        return result;
    }
    
    /// <summary>
    /// Retrieves upcoming Showings for a specific movie.
    /// </summary>
    /// <param name="movieId">The Movie ID.</param>
    /// <returns>
    /// A read-only list of <see cref="ShowingResponse"/>.
    /// Returns an empty list if no data is found or an error occurs.
    /// </returns>
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
    
    /// <summary>
    /// Creates a new Showing.
    /// </summary>
    /// <param name="movieId">The Movie ID.</param>
    /// <param name="auditoriumId">The Auditorium ID.</param>
    /// <param name="startsAt">The start time of the Showing.</param>
    /// <returns>
    /// True if the Showing was successfully created; otherwise false.
    /// </returns>
    public async Task<bool> AddShowingAsync(int movieId, int auditoriumId, DateTimeOffset startsAt)
    {
        var encodedStartsAt = Uri.EscapeDataString(startsAt.ToString("o"));
        var url = $"api/showings?movieId={movieId}&auditoriumId={auditoriumId}&startsAt={encodedStartsAt}";
 
        var response = await _http.PostAsync(url, null);
        return response.IsSuccessStatusCode;
    }
}
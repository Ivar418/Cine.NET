using System.Net.Http.Json;
using SharedLibrary.Domain.Entities;
using WA.Services.Http.Interfaces;

namespace WA.Services.Http;

public class AuditoriumApi : IAuditoriumApi
{
    private readonly HttpClient _http;
    private const string BasePath = "api/auditoriums";

    public AuditoriumApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Auditorium>> GetAllAuditoriumsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<Auditorium>>(BasePath);
        return result ?? new List<Auditorium>();
    }
}

using System.Net.Http.Json;
using SharedLibrary.DTOs.Requests;
using WA.Services.Http.Interfaces;

namespace WA.Services.Http;

public sealed class ArrangementApi : IArrangementApi
{
    private readonly HttpClient _http;

    public ArrangementApi(HttpClient http) => _http = http;

    public async Task CreateAsync(CreateArrangementRequest request, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/arrangements", request, ct);

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new Exception(error);
        }
    }
}
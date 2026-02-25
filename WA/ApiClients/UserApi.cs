using System.Net.Http.Json;
using SharedLibrary.DTOs.Responses;

namespace WA.ApiClients;

public sealed class UserApi : IUserApi
{
    private readonly HttpClient _http;

    public UserApi(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<UserResponse>> GetUsersAsync(CancellationToken ct = default)
    {
        var users = await _http.GetFromJsonAsync<List<UserResponse>>("api/users", ct);
        return users ?? [];
    }
}
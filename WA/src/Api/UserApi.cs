using System.Net.Http.Json;
using Cine.NET_WA.Dto;

namespace Cine.NET_WA.Api;

public sealed class UserApi : IUserApi
{
    private readonly HttpClient _http;

    public UserApi(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default)
    {
        var users = await _http.GetFromJsonAsync<List<UserDto>>("api/users", ct);
        return users ?? [];
    }
}
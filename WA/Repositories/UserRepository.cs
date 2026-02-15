using Cine.NET_WA.Api;
using Cine.NET_WA.Domain;
using Cine.NET_WA.Mapping;

namespace Cine.NET_WA.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IUserApi _api;

    public UserRepository(IUserApi api) => _api = api;

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct)
    {
        var dtos = await _api.GetUsersAsync(ct);
        return dtos.Select(UserMapper.ToDomain).ToList();
    }
}
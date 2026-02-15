using Cine.NET_WA.Domain;
using Cine.NET_WA.Repositories;

namespace Cine.NET_WA.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo) => _repo = repo;

    public Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken ct = default)
        => _repo.GetAllAsync(ct);
}
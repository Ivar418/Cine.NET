using Cine.NET_WA.Domain;

namespace Cine.NET_WA.Services;

public interface IUserService
{
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken ct = default);
}
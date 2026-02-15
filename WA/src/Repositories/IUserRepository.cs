using Cine.NET_WA.Domain;

namespace Cine.NET_WA.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
}
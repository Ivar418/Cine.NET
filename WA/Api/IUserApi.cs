using Cine.NET_WA.Dto;

namespace Cine.NET_WA.Api;

public interface IUserApi
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default);
}
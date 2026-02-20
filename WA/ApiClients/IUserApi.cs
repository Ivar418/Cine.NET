using SharedLibrary.DTOs.Responses;

namespace WA.ApiClients;

public interface IUserApi
{
    Task<IReadOnlyList<UserResponse>> GetUsersAsync(CancellationToken ct = default);
}
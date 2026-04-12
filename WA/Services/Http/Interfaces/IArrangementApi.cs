using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace WA.Services.Http.Interfaces;

public interface IArrangementApi
{
    Task CreateAsync(CreateArrangementRequest request, CancellationToken ct = default);
}
using SharedLibrary.DTOs.Responses;

namespace WA.Services.Http.Interfaces;

public interface IShowingApi
{
    Task<IReadOnlyList<ShowingsWithPricesResponse>> GetShowingsWithPricesAsync();
    Task<ShowingsWithPricesResponse?> GetShowingPricesAsync(int showingId);
}
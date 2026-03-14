using SharedLibrary.DTOs.Responses;

namespace WA.Services.Http.Interfaces;

public interface IShowingApi
{
    Task<IReadOnlyList<ShowingsWithPricesResponse>> GetShowingsWithPricesAsync();
    
    Task<ShowingResponse?> GetShowingByIdAsync(int id);
    
    Task<ShowingDisplayResponse?> GetShowingDisplayByIdAsync(int id);
}
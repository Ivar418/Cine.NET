using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces;

public interface IShowingService
{
    Task<List<ShowingsWithPricesResponse>> GetShowingsAsync();

    Task<ShowingsWithPricesResponse?> GetShowingAsync(int id);
    
    Task<List<ShowingResponse>> GetUpcomingShowingsByMovieIdAsync(int movieId);
}
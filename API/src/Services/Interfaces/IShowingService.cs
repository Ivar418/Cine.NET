using API.Domain.Common;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces;

public interface IShowingService
{
    Task<ResultOf<List<ShowingsWithPricesResponse>>> GetShowingsAsync();

    Task<ResultOf<ShowingsWithPricesResponse>> GetShowingAsync(int id);
    
    Task<ResultOf<IReadOnlyList<ShowingResponse>>> GetUpcomingShowingsByMovieIdAsync(int movieId);
}
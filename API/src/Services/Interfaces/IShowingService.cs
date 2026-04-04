using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Interfaces;

public interface IShowingService
{
    Task<ResultOf<List<ShowingsWithPricesResponse>>> GetShowingsAsync();

    Task<ResultOf<ShowingsWithPricesResponse>> GetShowingAsync(int id);
    Task<ResultOf<Showing>> GetFullShowingByIdAsync(int id);
    Task<ResultOf<ShowingStateDto>> GetShowingStateAsync(int id);
    Task<ResultOf<IReadOnlyList<ShowingResponse>>> GetUpcomingShowingsByMovieIdAsync(int movieId);
    Task<ResultOf<ICollection<ShowingDisplayResponse>>> GetShowingDisplayAsync(DateOnly? date = null);
    Task<ResultOf<Showing>> GetRandomShowingWithAmountOfSeatsAvailableAsync(int seatsNeededAmount);
}
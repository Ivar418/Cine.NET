using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace WA.Services.Http.Interfaces;

public interface IShowingApi
{
    Task<IReadOnlyList<ShowingsWithPricesResponse>> GetShowingsWithPricesAsync();
    Task<Showing?> GetShowingByIdAsync(int id);
    Task<ShowingDisplayResponse?> GetShowingDisplayByIdAsync(int id);
    Task<ShowingsWithPricesResponse?> GetShowingPricesAsync(int showingId);
    Task<IReadOnlyList<ShowingDisplayResponse>> GetShowingDisplayAsync(DateOnly? date = null);
    Task<IReadOnlyList<Showing>> GetUpcomingShowingsByMovieIdAsync(int movieId);
    Task<bool> AddShowingAsync(int movieId, int auditoriumId, DateTimeOffset startsAt);
}
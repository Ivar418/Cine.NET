using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Responses;

namespace API.Repositories.Interfaces
{
    public interface IShowingRepository
    {
        Task<ResultOf<Showing>> GetShowingAsync(int id);
        Task<ResultOf<ICollection<Showing>>> GetShowingsAsync();
        Task<Showing> AddShowingAsync(CreateShowingRequest Showing);
        Task<Showing> UpdateShowingAsync(Showing Showing);
        Task<ResultOf<Showing>> DeleteShowingByIdAsync(int ShowingId);
        Task<ResultOf<ShowingDisplayResponse>> GetShowingDisplayByIdAsync(int id);
        Task<ResultOf<ICollection<ShowingResponse>>> GetUpcomingShowingsByMovieIdAsync(int movieId, DateTimeOffset cutoff);
        Task<ResultOf<ICollection<ShowingDisplayResponse>>> GetShowingDisplayAsync(DateOnly? date = null);
    }
}

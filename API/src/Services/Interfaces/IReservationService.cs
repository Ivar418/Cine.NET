using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.Services.Interfaces
{
    public interface IReservationService
    {
        Task<SuggestResponse?> SuggestAsync(SuggestRequest req);
        Task<Reservation?> ConfirmAsync(Guid suggestionId);
        Task<Reservation?> CancelAsync(Guid reservationId);
        Task<HashSet<string>> GetOccupiedKeysAsync(int showingId);
    }
}

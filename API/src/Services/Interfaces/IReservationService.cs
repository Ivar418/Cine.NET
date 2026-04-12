using API.Domain.Common;
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
        Task<Reservation> UpdateReservationStatusAsync(Guid id, string status); 
        Task<ResultOf<Reservation>> GetReservationByIdAsync(Guid id);
        Task<List<Reservation>> GetReservationByShowingAsync(int showtimeId);
        Task<Reservation?> UpdateReservationSeatsAsync(Guid id, IEnumerable<SeatInfo> seats);
    }
}

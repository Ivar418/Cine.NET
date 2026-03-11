using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetReservationByShowingAsync(int showtimeId);
        Task<ResultOf<Reservation>> GetReservationByIdAsync(Guid id);
        Task<Reservation> CreateReservationAsync(int showtimeId, IEnumerable<SeatInfo> seats, string status);
        Task<Reservation> UpdateReservationStatusAsync(Guid id, string status);

    }
}

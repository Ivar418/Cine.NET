using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Services.Interfaces
{
    public interface IReservationService
    {
            Task<SuggestResponse?> SuggestAsync(SuggestRequest req);
            Task<Reservation?> ConfirmAsync(Guid suggestionId);
            Task<Reservation?> CancelAsync(Guid reservationId);
    }
}

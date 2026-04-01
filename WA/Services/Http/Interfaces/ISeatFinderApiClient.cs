using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace WA.Services.Http;

public interface ISeatFinderApiClient
{
    // ── Auditoriums ──────────────────────────────────────────────────────────
    Task<List<Auditorium>?> GetAuditoriumsAsync();
    Task<Auditorium?> GetAuditoriumAsync(int id);
    Task<Auditorium?> CreateAuditoriumAsync(CreateAuditoriumRequest req);
    Task<Auditorium?> UpdateAuditoriumAsync(int id, UpdateAuditoriumRequest req);
    Task<bool> DeleteAuditoriumAsync(int id);

    // ── Showings ─────────────────────────────────────────────────────────────
    Task<List<Showing>?> GetShowingsAsync();
    Task<ShowingStateDto?> GetShowingStateAsync(int id);
    Task<Showing?> CreateShowingAsync(CreateShowingRequest req);
    Task<bool> DeleteShowingAsync(int id);

    // ── Reservations ──────────────────────────────────────────────────────────
    Task<SuggestResponse?> SuggestAsync(SuggestRequest req);
    Task<Reservation?> ConfirmAsync(Guid suggestionId);
    Task<Reservation?> CancelAsync(Guid reservationId);
    Task<Reservation?> UpdateReservationSeatsAsync(Guid suggestionId, IEnumerable<SeatInfo> seats);
}
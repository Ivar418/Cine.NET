using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace WA.Services.Http;

public interface ISeatFinderApiClient
{
    // ── Auditoriums ──────────────────────────────────────────────────────────
    Task<List<AuditoriumDto>?> GetAuditoriumsAsync();
    Task<AuditoriumDto?> GetAuditoriumAsync(int id);
    Task<AuditoriumDto?> CreateAuditoriumAsync(CreateAuditoriumRequest req);
    Task<AuditoriumDto?> UpdateAuditoriumAsync(int id, UpdateAuditoriumRequest req);
    Task<bool> DeleteAuditoriumAsync(int id);

    // ── Showings ─────────────────────────────────────────────────────────────
    Task<List<Showing>?> GetShowingsAsync();
    Task<ShowingStateDto?> GetShowingStateAsync(int id);
    Task<Showing?> CreateShowingAsync(CreateShowingRequest req);
    Task<bool> DeleteShowingAsync(int id);

    // ── Reservations ──────────────────────────────────────────────────────────
    Task<SuggestResponse?> SuggestAsync(SuggestRequest req);
    Task<ReservationDto?> ConfirmAsync(Guid suggestionId);
    Task<ReservationDto?> CancelAsync(Guid reservationId);
}
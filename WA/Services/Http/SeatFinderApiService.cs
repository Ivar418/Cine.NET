using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using System.Net.Http.Json;

namespace WA.Services.Http
{
        public sealed class SeatFinderApiService(HttpClient http)
        {
            // ── Auditoriums ─────────────────────────────────────────────────────────────
            public async Task<List<AuditoriumDto>?> GetAuditoriumsAsync() =>
                await http.GetFromJsonAsync<List<AuditoriumDto>>("api/auditoriums");

            public async Task<AuditoriumDto?> GetAuditoriumAsync(int id) =>
                await http.GetFromJsonAsync<AuditoriumDto>($"api/auditoriums/{id}");

            public async Task<AuditoriumDto?> CreateAuditoriumAsync(CreateAuditoriumRequest req)
            {
                var r = await http.PostAsJsonAsync("api/auditoriums", req);
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<AuditoriumDto>() : null;
            }

            public async Task<AuditoriumDto?> UpdateAuditoriumAsync(int id, UpdateAuditoriumRequest req)
            {
                var r = await http.PutAsJsonAsync($"api/auditoriums/{id}", req);
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<AuditoriumDto>() : null;
            }

            public async Task<bool> DeleteAuditoriumAsync(int id) =>
                (await http.DeleteAsync($"api/auditoriums/{id}")).IsSuccessStatusCode;

            // ── Showings ─────────────────────────────────────────────────────────
            public async Task<List<Showing>?> GetShowingsAsync() =>
                await http.GetFromJsonAsync<List<Showing>>("api/showings");

            public async Task<ShowingStateDto?> GetShowingStateAsync(int id) =>
                await http.GetFromJsonAsync<ShowingStateDto>($"api/showings/{id}/state");

            public async Task<Showing?> CreateShowingAsync(CreateShowingRequest req)
            {
                var r = await http.PostAsJsonAsync("api/showings", req);
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<Showing>() : null;
            }

            public async Task<bool> DeleteShowingAsync(int id) =>
                (await http.DeleteAsync($"api/showings/{id}")).IsSuccessStatusCode;

            // ── Reservations ──────────────────────────────────────────────────────
            public async Task<SuggestResponse?> SuggestAsync(SuggestRequest req)
            {
                var r = await http.PostAsJsonAsync("api/reservations/suggest", req);
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<SuggestResponse>() : null;
            }

            public async Task<ReservationDto?> ConfirmAsync(Guid suggestionId)
            {
                var r = await http.PostAsJsonAsync("api/reservations/confirm", new ConfirmRequest(suggestionId));
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<ReservationDto>() : null;
            }

            public async Task<ReservationDto?> CancelAsync(Guid reservationId)
            {
                var r = await http.PostAsJsonAsync("api/reservations/cancel", new CancelRequest(reservationId));
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<ReservationDto>() : null;
            }
        }
}

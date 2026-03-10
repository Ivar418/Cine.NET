using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using System.Net.Http.Json;

namespace WA.Services.Http
{
        public sealed class CinemaApiService(HttpClient http)
        {
            // ── Halls ─────────────────────────────────────────────────────────────
            public Task<List<HallDto>?> GetHallsAsync() =>
                http.GetFromJsonAsync<List<HallDto>>("api/halls");

            public Task<HallDto?> GetHallAsync(int id) =>
                http.GetFromJsonAsync<HallDto>($"api/halls/{id}");

            public async Task<HallDto?> CreateHallAsync(CreateHallRequest req)
            {
                var r = await http.PostAsJsonAsync("api/halls", req);
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<HallDto>() : null;
            }

            public async Task<HallDto?> UpdateHallAsync(int id, UpdateHallRequest req)
            {
                var r = await http.PutAsJsonAsync($"api/halls/{id}", req);
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<HallDto>() : null;
            }

            public async Task<bool> DeleteHallAsync(int id) =>
                (await http.DeleteAsync($"api/halls/{id}")).IsSuccessStatusCode;

            // ── Showtimes ─────────────────────────────────────────────────────────
            public Task<List<Showtime>?> GetShowtimesAsync() =>
                http.GetFromJsonAsync<List<Showtime>>("api/showtimes");

            public Task<ShowtimeStateDto?> GetShowtimeStateAsync(int id) =>
                http.GetFromJsonAsync<ShowtimeStateDto>($"api/showtimes/{id}/state");

            public async Task<Showtime?> CreateShowtimeAsync(CreateShowtimeRequest req)
            {
                var r = await http.PostAsJsonAsync("api/showtimes", req);
                return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<Showtime>() : null;
            }

            public async Task<bool> DeleteShowtimeAsync(int id) =>
                (await http.DeleteAsync($"api/showtimes/{id}")).IsSuccessStatusCode;

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

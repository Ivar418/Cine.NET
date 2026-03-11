using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using System.Net.Http.Json;

namespace WA.Services.Http
{
    public class SeatFinderApiService : ISeatFinderApiClient
    {
        private readonly HttpClient _http;

        public SeatFinderApiService(HttpClient http)
        {
            _http = http;
        }

        // ── Auditoriums ──────────────────────────────────────────────────────────

        public async Task<List<AuditoriumDto>?> GetAuditoriumsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<AuditoriumDto>>("api/auditoriums");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] GetAuditoriums failed: {ex.Message}");
                return null;
            }
        }

        public async Task<AuditoriumDto?> GetAuditoriumAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<AuditoriumDto>($"api/auditoriums/{id}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] GetAuditorium({id}) failed: {ex.Message}");
                return null;
            }
        }

        public async Task<AuditoriumDto?> CreateAuditoriumAsync(CreateAuditoriumRequest req)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auditoriums", req);
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<AuditoriumDto>()
                    : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] CreateAuditorium failed: {ex.Message}");
                return null;
            }
        }

        public async Task<AuditoriumDto?> UpdateAuditoriumAsync(int id, UpdateAuditoriumRequest req)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"api/auditoriums/{id}", req);
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<AuditoriumDto>()
                    : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] UpdateAuditorium({id}) failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAuditoriumAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/auditoriums/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] DeleteAuditorium({id}) failed: {ex.Message}");
                return false;
            }
        }

        // ── Showings ─────────────────────────────────────────────────────────────

        public async Task<List<Showing>?> GetShowingsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Showing>>("api/showings");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] GetShowings failed: {ex.Message}");
                return null;
            }
        }

        public async Task<ShowingStateDto?> GetShowingStateAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<ShowingStateDto>($"api/showings/{id}/state");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] GetShowingState({id}) failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Showing?> CreateShowingAsync(CreateShowingRequest req)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/showings", req);
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<Showing>()
                    : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] CreateShowing failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteShowingAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/showings/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] DeleteShowing({id}) failed: {ex.Message}");
                return false;
            }
        }

        // ── Reservations ──────────────────────────────────────────────────────────

        public async Task<SuggestResponse?> SuggestAsync(SuggestRequest req)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/reservations/suggest", req);
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<SuggestResponse>()
                    : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] Suggest failed: {ex.Message}");
                return null;
            }
        }

        public async Task<ReservationDto?> ConfirmAsync(Guid suggestionId)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/reservations/confirm", new ConfirmRequest(suggestionId));
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<ReservationDto>()
                    : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] Confirm({suggestionId}) failed: {ex.Message}");
                return null;
            }
        }

        public async Task<ReservationDto?> CancelAsync(Guid reservationId)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/reservations/cancel", new CancelRequest(reservationId));
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<ReservationDto>()
                    : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SeatFinderApiClient] Cancel({reservationId}) failed: {ex.Message}");
                return null;
            }
        }
    }
}

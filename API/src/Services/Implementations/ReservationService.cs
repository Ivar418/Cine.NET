using API.Infrastructure.Database;
using API.src.Repositories.Implementations;
using API.src.Repositories.Interfaces;
using API.src.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.Logic.Algorithm;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.src.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IShowingRepository _showingRepository;
        private readonly IAuditoriumRepository _auditoriumRepository;
        private readonly ApiDbContext _db;
        public ReservationService() { 
            _auditoriumRepository = new AuditoriumRepository(_db);
            _reservationRepository = new ReservationRepository(_db);
            _showingRepository = new ShowingRepository(_db);
        }

        Task<Reservation?> IReservationService.CancelAsync(Guid reservationId)
        {
            throw new NotImplementedException();
        }

        Task<Reservation?> IReservationService.ConfirmAsync(Guid suggestionId)
        {
            throw new NotImplementedException();
        }

        async Task<SuggestResponse?> IReservationService.SuggestAsync(SuggestRequest req)
        {
            var showingResult = await _showingRepository.GetShowingAsync(req.ShowingId);
            if (showingResult.Value is null) return null;

            // Use the frozen snapshot rows.
            var rows = showingResult.Value.GetLayoutSnapshot();
            var occupied = await _reservationRepository.GetOccupiedKeysAsync(req.ShowingId);
            var request = new ReservationRequest(req.NormalCount, req.WheelchairCount);
            var best = SeatFinder.FindBest(rows, occupied, request);

            if (best is null)
            {
                return new SuggestResponse(Guid.Empty,
                    new List<SeatInfo>(),
                    false
                );
            }

            var res = await _reservationRepository.CreateReservationAsync(req.ShowingId, best, "Pending");
            return new SuggestResponse(
                res.Id,
                best,
                true
            );
        }
    }
}

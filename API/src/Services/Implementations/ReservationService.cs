using API.Infrastructure.Database;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.Logic.Algorithm;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Domain.Common;
using API.Services.Interfaces;
using API.src.Services.Interfaces;
using API.Repositories.Interfaces;

namespace API.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IShowingService _showingService;
        private readonly IReservationRepository _reservationRepository;
        public ReservationService(IShowingService showingService, IReservationRepository reservationRepository)
        {
            _showingService = showingService;
            _reservationRepository = reservationRepository;
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
            var showingResult = _showingService.GetShowingAsync(req.ShowingId);
            if (showingResult == null)
            {
                Console.WriteLine("Showing was not found!");
                return new SuggestResponse(Guid.Empty,
                    new List<SeatInfo>(),
                    false
                );
            }

            // Use the frozen snapshot rows.
            var rows = showingResult.GetLayoutSnapshot();
            var occupied = _reservationRepository.GetOccupiedKeysAsync(req.ShowingId);
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

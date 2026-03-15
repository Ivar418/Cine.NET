using API.Services;
using API.src.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.Logic.Algorithm;
using System.Threading.Tasks;

namespace API.src.Mappers
{
    public static class ShowingMapper
    {
        // Synchronous wrapper for compatibility
        public static ShowingStateDto ToStateDto(this Showing showing, IReservationRepository reservationRepository)
        {
            return ToStateDtoAsync(showing, reservationRepository).GetAwaiter().GetResult();
        }

        // Async implementation that properly awaits the repository call
        public static async Task<ShowingStateDto> ToStateDtoAsync(this Showing showing, IReservationRepository reservationRepository)
        {
            // 1. ShowingDto
            var showingDto = new ShowingDto(
                showing.Id,
                showing.Movie,
                new AuditoriumDto(
                    showing.Auditorium.Id,
                    showing.Auditorium.Name,
                    showing.Auditorium.GetRows()
                ),
                showing.StartsAt
            );

            var rows = showing.GetLayoutSnapshot();
            var allSeats = ZoneCalculator.BuildSeatMap(rows);
            var occupied = await reservationRepository.GetOccupiedKeysAsync(showing.Id) ?? new HashSet<string>();


            return new ShowingStateDto(
                showingDto,
                allSeats,
                occupied
            );
        }
    }

}

using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.Domain.Common;
using API.Infrastructure.Database;
using API.src.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Responses;

namespace API.src.Repositories.Implementations
{
    public class ShowingRepository : IShowingRepository
    {
        private readonly ApiDbContext _db;

        public ShowingRepository(ApiDbContext db)
        {
            _db = db;
        }
        async Task<Showing> IShowingRepository.AddShowingAsync(CreateShowingRequest Showing)
        {
            Console.WriteLine($"Adding Showing of movie: {Showing.MovieId}");
            Showing newShowing = new Showing
            {
                MovieId = Showing.MovieId,
                AuditoriumId = Showing.AuditoriumId,
                StartsAt = Showing.StartsAt
            };
            Auditorium auditorium = _db.Auditoriums.FirstOrDefault(a => a.Id == Showing.AuditoriumId);

            if (auditorium == null)
            {
                throw new Exception($"Auditorium with id {Showing.AuditoriumId} not found.");
            }
            newShowing.SetLayoutSnapshot(auditorium.GetRows());

            var result = await _db.Showings.AddAsync(newShowing);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        async Task<ResultOf<Showing>> IShowingRepository.DeleteShowingByIdAsync(int ShowingId)
        {
            throw new NotImplementedException();
        }

        async Task<ResultOf<Showing>> IShowingRepository.GetShowingAsync(int id)
        {
            var Showing = await _db.Showings.FindAsync(id);
            return Showing == null ? ResultOf<Showing>.Failure("Auditorium not found") : ResultOf<Showing>.Success(Showing);
        }

        async Task<ResultOf<ICollection<Showing>>> IShowingRepository.GetShowingsAsync()
        {
            try
            {
                var Showings = await _db.Showings.ToListAsync();

                return ResultOf<ICollection<Showing>>.Success(Showings);
            }
            catch (Exception e)
            {
                return ResultOf<ICollection<Showing>>.Failure(e.Message);
            }
        }

        /*
         PSEUDOCODE / PLAN:
         - Retrieve the showing by id (already done earlier).
         - Ensure showing and showing.Movie runtime exist; return failure results if not.
         - Get the auditorium layout snapshot via `showing.GetLayoutSnapshot()` which returns List<RowConfig>.
         - Convert that list of RowConfig into a List<SeatInfo> (AllSeats):
           - For each row (index), iterate seats from 0..row.Seats-1
           - Create a SeatInfo for each seat with Row, Col, VirtualCol and sensible defaults for Type and Category.
           - (We avoid inventing complex wheelchair-placement logic here; this produces a seat entry per seat count.)
         - Build ShowingStateDto with Showing info, AllSeats, and OccupiedKeys (keeps the original occupied-keys extraction).
         - Return a successful ResultOf<ShowingStateDto> with the constructed DTO.
        */

        Task<ResultOf<ShowingStateDto>> IShowingRepository.GetShowingStateAsync(int id)
        {
            throw new NotImplementedException();
        }

        async Task<Showing> IShowingRepository.UpdateShowingAsync(Showing Showing)
        {
            throw new NotImplementedException();
        }
        
        async Task<ResultOf<ShowingDisplayResponse>> IShowingRepository.GetShowingDisplayByIdAsync(int id)
        {
            try
            {
                var showing = await _db.Showings
                    .Where(s => s.Id == id)
                    .Select(s => new ShowingDisplayResponse
                    {
                        Id = s.Id,
                        MovieId = s.MovieId,
                        AuditoriumId = s.AuditoriumId,
                        MovieTitle = s.Movie.Title,
                        AuditoriumName = s.Auditorium.Name,
                        Is3D = s.IsThreeD,
                        StartsAt = s.StartsAt
                    })
                    .FirstOrDefaultAsync();

                return showing == null
                    ? ResultOf<ShowingDisplayResponse>.Failure("Showing not found")
                    : ResultOf<ShowingDisplayResponse>.Success(showing);
            }
            catch (Exception e)
            {
                return ResultOf<ShowingDisplayResponse>.Failure(e.Message);
            }
        }
        
        async Task<ResultOf<ICollection<ShowingDisplayResponse>>> IShowingRepository.GetShowingDisplayAsync()
        {
            try
            {
                var showings = await _db.Showings
                    .Select(s => new ShowingDisplayResponse
                    {
                        Id = s.Id,
                        MovieId = s.MovieId,
                        AuditoriumId = s.AuditoriumId,
                        MovieTitle = s.Movie.Title,
                        AuditoriumName = s.Auditorium.Name,
                        Is3D = s.IsThreeD,
                        StartsAt = s.StartsAt
                    })
                    .ToListAsync();
 
                return ResultOf<ICollection<ShowingDisplayResponse>>.Success(showings);
            }
            catch (Exception e)
            {
                return ResultOf<ICollection<ShowingDisplayResponse>>.Failure(e.Message);
            }
        }
    }
}

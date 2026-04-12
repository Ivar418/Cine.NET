using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using API.Mappers;
using SharedLibrary.DTOs.Responses;
using API.src.Repositories.Implementations;

namespace API.Repositories.Implementations
{
    public class ShowingRepository : IShowingRepository
    {
        private readonly ApiDbContext _db;

        public ShowingRepository(ApiDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates and persists a showing based on the provided creation request.
        /// </summary>
        /// <param name="showingRequest">The showing creation payload.</param>
        /// <returns>The persisted <see cref="Showing"/> entity.</returns>
        async Task<Showing> IShowingRepository.AddShowingAsync(CreateShowingRequest showingRequest)
        {
            Console.WriteLine($"Adding Showing of movie: {showingRequest.MovieId}");
            Showing newShowing = new Showing
            {
                MovieId = showingRequest.MovieId,
                AuditoriumId = showingRequest.AuditoriumId,
                StartsAt = showingRequest.StartsAt,
                IsThreeD = showingRequest.Is3D,
            };
            Auditorium auditorium = _db.Auditoriums.FirstOrDefault(a => a.Id == showingRequest.AuditoriumId);
            Movie movie = _db.Movies.FirstOrDefault(m => m.Id == showingRequest.MovieId);
            if (auditorium == null)
            {
                throw new Exception($"Auditorium with id {showingRequest.AuditoriumId} not found.");
            }
            newShowing.SetLayoutSnapshot(auditorium.GetRows());

            var result = await _db.Showings.AddAsync(newShowing);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ResultOf<Showing>> DeleteShowingByIdAsync(int ShowingId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a showing by identifier including related movie and auditorium data.
        /// </summary>
        /// <param name="id">The showing identifier.</param>
        /// <returns>
        /// A <see cref="ResultOf{T}"/> containing the showing when found,
        /// or a failure result with <c>NotFound</c> when no matching showing exists.
        /// </returns>
        public async Task<ResultOf<Showing>> GetShowingAsync(int id)
        {
            var showing = await _db.Showings
                .Include(s => s.Movie)
                .Include(s => s.Auditorium)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            return showing == null
                ? ResultOf<Showing>.Failure("NotFound")
                : ResultOf<Showing>.Success(showing);
        }

        /// <summary>
        /// Retrieves all showings including related movie and auditorium data.
        /// </summary>
        /// <returns>
        /// A <see cref="ResultOf{T}"/> containing all showings on success,
        /// or a failure result when retrieval fails.
        /// </returns>
        public async Task<ResultOf<ICollection<Showing>>> GetShowingsAsync()
        {
            try
            {
                var showings = await _db.Showings
                    .Include(s => s.Movie)
                    .Include(s => s.Auditorium)
                    .ToListAsync();

                return ResultOf<ICollection<Showing>>.Success(showings);
            }
            catch (Exception e)
            {
                return ResultOf<ICollection<Showing>>.Failure(e.Message);
            }
        }
        
        public async Task<Showing> UpdateShowingAsync(Showing Showing)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Retrieves display data for a single showing.
        /// </summary>
        /// <param name="id">The showing identifier.</param>
        /// <returns>
        /// A <see cref="ResultOf{T}"/> containing the projected showing display response,
        /// or a failure result when the showing does not exist or retrieval fails.
        /// </returns>
        public async Task<ResultOf<ShowingDisplayResponse>> GetShowingDisplayByIdAsync(int id)
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
                        Runtime = s.Movie.Runtime,
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
        
        /// <summary>
        /// Retrieves all upcoming showings for a specific movie from the database, projected directly
        /// to <see cref="ShowingResponse"/>. Only showings with a start time greater than
        /// <paramref name="cutoff"/> are included, ordered by start time ascending.
        /// </summary>
        /// <param name="movieId">The internal ID of the movie to retrieve showings for.</param>
        /// <param name="cutoff">
        /// The earliest allowed start time. Showings starting after this value will be included.
        /// Passed in from the service layer to keep business rules out of the repository.
        /// </param>
        /// <returns>
        /// A <see cref="ResultOf{T}"/> containing a collection of <see cref="ShowingResponse"/> on success,
        /// or a failure with the exception message if the database query fails.
        /// </returns>
        async Task<ResultOf<ICollection<ShowingResponse>>> IShowingRepository.GetUpcomingShowingsByMovieIdAsync(int movieId, DateTimeOffset cutoff)
        {
            try
            {
                var showings = await _db.Showings
                    .Where(s => s.MovieId == movieId && s.StartsAt > cutoff)
                    .OrderBy(s => s.StartsAt)
                    .Select(s => new ShowingResponse
                    {
                        Id                       = s.Id,
                        MovieId                  = s.MovieId,
                        AuditoriumId             = s.AuditoriumId,
                        Is3D                     = s.IsThreeD,
                        StartsAt                 = s.StartsAt,
                        AuditoriumLayoutSnapshot = s.AuditoriumLayoutSnapshot
                    })
                    .ToListAsync();

                return ResultOf<ICollection<ShowingResponse>>.Success(showings);
            }
            catch (Exception e)
            {
                return ResultOf<ICollection<ShowingResponse>>.Failure(e.Message);
            }
        }

        /// <summary>
        /// Retrieves showing display data, optionally filtered by calendar date.
        /// </summary>
        /// <param name="date">Optional date filter applied to showing start dates.</param>
        /// <returns>
        /// A <see cref="ResultOf{T}"/> containing projected showing display responses,
        /// or a failure result when retrieval fails.
        /// </returns>
        async Task<ResultOf<ICollection<ShowingDisplayResponse>>> IShowingRepository.GetShowingDisplayAsync(DateOnly? date)
        {
            try
            {
                var query = _db.Showings.AsQueryable();

                if (date.HasValue)
                    query = query.Where(s => s.StartsAt.Date == date.Value.ToDateTime(TimeOnly.MinValue).Date);

                var showings = await query
                    .Select(s => new ShowingDisplayResponse
                    {
                        Id = s.Id,
                        MovieId = s.MovieId,
                        AuditoriumId = s.AuditoriumId,
                        MovieTitle = s.Movie.Title,
                        AuditoriumName = s.Auditorium.Name,
                        Runtime = s.Movie.Runtime,
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

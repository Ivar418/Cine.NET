using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Repositories.Implementations;

public class ReservationRepository : IReservationRepository
{
    private readonly ApiDbContext _db;

    public ReservationRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves reservations for a showing ordered by creation time descending.
    /// </summary>
    /// <param name="showingId">The showing identifier.</param>
    /// <returns>A list of reservations for the specified showing.</returns>
    public Task<List<Reservation>> GetReservationByShowingAsync(int showingId) =>
        _db.Reservations.AsNoTracking()
          .Where(r => r.ShowingId == showingId)
          .OrderByDescending(r => r.CreatedAt)
          .ToListAsync();

    /// <summary>
    /// Retrieves a reservation by its identifier.
    /// </summary>
    /// <param name="id">The reservation identifier.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the reservation on success,
    /// or a failure result when the reservation does not exist.
    /// </returns>
    public async Task<ResultOf<Reservation>> GetReservationByIdAsync(Guid id) {
        var Reservation = await _db.Reservations.FindAsync(id);
        return Reservation == null ? ResultOf<Reservation>.Failure("Reservation not found") : ResultOf<Reservation>.Success(Reservation);
    }

    /// <summary>
    /// Creates and persists a reservation with seats and status.
    /// </summary>
    /// <param name="showtimeId">The showing identifier.</param>
    /// <param name="seats">The seat selection to assign.</param>
    /// <param name="status">The initial reservation status.</param>
    /// <returns>The persisted reservation entity.</returns>
    public async Task<Reservation> CreateReservationAsync(int showtimeId, IEnumerable<SeatInfo> seats, string status)
    {
        var res = new Reservation { ShowingId = showtimeId, Status = status };
        res.SetSeats(seats);
        _db.Reservations.Add(res);
        await _db.SaveChangesAsync();

        return res;
    }

    /// <summary>
    /// Updates the status of an existing reservation.
    /// </summary>
    /// <param name="id">The reservation identifier.</param>
    /// <param name="status">The new reservation status.</param>
    /// <returns>The updated reservation, or <c>null</c> when not found.</returns>
    public async Task<Reservation?> UpdateReservationStatusAsync(Guid id, string status)
    {
        var res = await _db.Reservations.FindAsync([id]);
        if (res is null) return null;
        res.Status = status;
        await _db.SaveChangesAsync();
        return res;
    }

    /// <summary>
    /// Updates the seat selection of an existing reservation.
    /// </summary>
    /// <param name="id">The reservation identifier.</param>
    /// <param name="seats">The replacement seat set.</param>
    /// <returns>The updated reservation, or <c>null</c> when not found.</returns>
    public async Task<Reservation?> UpdateReservationSeatsAsync(Guid id, IEnumerable<SeatInfo> seats)
    {
        var res = await _db.Reservations.FindAsync(id);
        if (res is null) return null;
        res.SetSeats(seats);
        await _db.SaveChangesAsync();
        return res;
    }

    /// <summary>
    /// Retrieves occupied seat keys for reservations that are pending or confirmed.
    /// </summary>
    /// <param name="showingId">The showing identifier.</param>
    /// <returns>A set of occupied seat keys in <c>row-column</c> format.</returns>
    public async Task<HashSet<string>> GetOccupiedKeysAsync(int showingId)
    {
        var reservations = await _db.Reservations
            .AsNoTracking()
            .Where(r => r.ShowingId == showingId
                     && (r.Status == "Confirmed" || r.Status == "Pending"))
            .ToListAsync();

        return reservations
            .SelectMany(r => r.GetSeats())
            .Select(s => $"{s.Row}-{s.Col}")
            .ToHashSet();
    }
}
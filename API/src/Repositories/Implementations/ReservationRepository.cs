using API.Domain.Common;
using API.Infrastructure.Database;
using API.src.Repositories.Interfaces;
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

    public Task<List<Reservation>> GetReservationByShowingAsync(int showingId) =>
        _db.Reservations.AsNoTracking()
          .Where(r => r.ShowingId == showingId)
          .OrderByDescending(r => r.CreatedAt)
          .ToListAsync();

    public async Task<ResultOf<Reservation>> GetReservationByIdAsync(Guid id) {
        var Reservation = await _db.Reservations.FindAsync(id);
        return Reservation == null ? ResultOf<Reservation>.Failure("Reservation not found") : ResultOf<Reservation>.Success(Reservation);
    }

    public async Task<Reservation> CreateReservationAsync(int showtimeId, IEnumerable<SeatInfo> seats, string status)
    {
        var res = new Reservation { ShowingId = showtimeId, Status = status };
        res.SetSeats(seats);
        _db.Reservations.Add(res);
        await _db.SaveChangesAsync();
        return res;
    }

    public async Task<Reservation?> UpdateReservationStatusAsync(Guid id, string status)
    {
        var res = await _db.Reservations.FindAsync([id]);
        if (res is null) return null;
        res.Status = status;
        await _db.SaveChangesAsync();
        return res;
    }
}
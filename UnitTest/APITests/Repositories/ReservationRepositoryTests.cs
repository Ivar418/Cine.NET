using API.src.Repositories.Implementations;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.APITests.Repositories;

public class ReservationRepositoryTests
{
    private static Reservation BuildReservation(
        Guid? id = null,
        int showingId = 1,
        string status = "Pending",
        DateTimeOffset? createdAt = null,
        params SeatInfo[] seats)
    {
        var reservation = new Reservation
        {
            Id = id ?? Guid.NewGuid(),
            ShowingId = showingId,
            Status = status,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow
        };

        reservation.SetSeats(seats.Length == 0
            ? new[] { new SeatInfo(1, 1, 1, SeatType.Normal, 1) }
            : seats);

        return reservation;
    }

    // -------------------------------------------------------
    // GetReservationByShowingAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetReservationByShowingAsync_WhenExists_ReturnsOrderedReservations()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetReservationByShowingAsync_WhenExists_ReturnsOrderedReservations));

        var oldest = BuildReservation(showingId: 7, createdAt: DateTimeOffset.UtcNow.AddMinutes(-15));
        var newest = BuildReservation(showingId: 7, createdAt: DateTimeOffset.UtcNow.AddMinutes(-1));
        var otherShowing = BuildReservation(showingId: 8, createdAt: DateTimeOffset.UtcNow);

        db.Reservations.AddRange(oldest, newest, otherShowing);
        await db.SaveChangesAsync();

        var repo = new ReservationRepository(db);

        var result = await repo.GetReservationByShowingAsync(7);

        Assert.Equal(2, result.Count);
        Assert.Equal(newest.Id, result[0].Id);
        Assert.Equal(oldest.Id, result[1].Id);
    }

    [Fact]
    public async Task GetReservationByShowingAsync_WhenNotFound_ReturnsEmptyList()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetReservationByShowingAsync_WhenNotFound_ReturnsEmptyList));
        var repo = new ReservationRepository(db);

        var result = await repo.GetReservationByShowingAsync(404);

        Assert.Empty(result);
    }

    // -------------------------------------------------------
    // GetReservationByIdAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetReservationByIdAsync_WhenExists_ReturnsSuccess()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetReservationByIdAsync_WhenExists_ReturnsSuccess));

        var reservation = BuildReservation();
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        var repo = new ReservationRepository(db);

        var result = await repo.GetReservationByIdAsync(reservation.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(reservation.Id, result.Value!.Id);
    }

    [Fact]
    public async Task GetReservationByIdAsync_WhenNotExists_ReturnsFailure()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetReservationByIdAsync_WhenNotExists_ReturnsFailure));
        var repo = new ReservationRepository(db);

        var result = await repo.GetReservationByIdAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Reservation not found", result.Error);
    }

    // -------------------------------------------------------
    // CreateReservationAsync
    // -------------------------------------------------------

    [Fact]
    public async Task CreateReservationAsync_CreatesReservation()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(CreateReservationAsync_CreatesReservation));
        var repo = new ReservationRepository(db);

        var seats = new[]
        {
            new SeatInfo(2, 3, 3, SeatType.Normal, 2),
            new SeatInfo(2, 4, 4, SeatType.Normal, 2)
        };

        var result = await repo.CreateReservationAsync(12, seats, "Pending");

        Assert.Equal(12, result.ShowingId);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(2, result.GetSeats().Count);
        Assert.Single(db.Reservations);
    }

    // -------------------------------------------------------
    // UpdateReservationStatusAsync
    // -------------------------------------------------------

    [Fact]
    public async Task UpdateReservationStatusAsync_WhenExists_UpdatesStatus()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(UpdateReservationStatusAsync_WhenExists_UpdatesStatus));

        var reservation = BuildReservation(status: "Pending");
        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        var repo = new ReservationRepository(db);

        var result = await repo.UpdateReservationStatusAsync(reservation.Id, "Confirmed");

        Assert.NotNull(result);
        Assert.Equal("Confirmed", result.Status);
    }

    [Fact]
    public async Task UpdateReservationStatusAsync_WhenNotExists_ReturnsNull()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(UpdateReservationStatusAsync_WhenNotExists_ReturnsNull));
        var repo = new ReservationRepository(db);

        var result = await repo.UpdateReservationStatusAsync(Guid.NewGuid(), "Confirmed");

        Assert.Null(result);
    }

    // -------------------------------------------------------
    // GetOccupiedKeysAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetOccupiedKeysAsync_ReturnsOnlyPendingAndConfirmedSeats()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetOccupiedKeysAsync_ReturnsOnlyPendingAndConfirmedSeats));

        var pending = BuildReservation(
            showingId: 50,
            status: "Pending",
            seats: new[]
            {
                new SeatInfo(1, 1, 1, SeatType.Normal, 1),
                new SeatInfo(1, 2, 2, SeatType.Normal, 1)
            });

        var confirmed = BuildReservation(
            showingId: 50,
            status: "Confirmed",
            seats: new[]
            {
                new SeatInfo(1, 2, 2, SeatType.Normal, 1),
                new SeatInfo(2, 1, 1, SeatType.Normal, 1)
            });

        var cancelled = BuildReservation(
            showingId: 50,
            status: "Cancelled",
            seats: new[] { new SeatInfo(9, 9, 9, SeatType.Normal, 1) });

        db.Reservations.AddRange(pending, confirmed, cancelled);
        await db.SaveChangesAsync();

        var repo = new ReservationRepository(db);

        var result = await repo.GetOccupiedKeysAsync(50);

        Assert.Equal(3, result.Count);
        Assert.Contains("1-1", result);
        Assert.Contains("1-2", result);
        Assert.Contains("2-1", result);
        Assert.DoesNotContain("9-9", result);
    }
}



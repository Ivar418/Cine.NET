using API.Repositories.Implementations;
using SharedLibrary.Domain.Entities;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.APITests.Repositories;

public class TicketRepositoryTests
{
    private static Movie BuildMovie(int id = 1)
    {
        return new Movie
        {
            Id = id,
            Title = $"Movie {id}",
            TmdbId = id,
            InformationLanguage = "en",
            RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+00:00"
        };
    }

    private static Auditorium BuildAuditorium(int id = 1)
    {
        return new Auditorium
        {
            Id = id,
            Name = $"Room {id}"
        };
    }

    private static Showing BuildShowing(int id, int movieId, int auditoriumId)
    {
        return new Showing
        {
            Id = id,
            MovieId = movieId,
            AuditoriumId = auditoriumId,
            StartsAt = DateTimeOffset.UtcNow.AddHours(1)
        };
    }

    private static Ticket BuildTicket(int id, int showingId, string seat = "A1")
    {
        return new Ticket
        {
            Id = id,
            ShowingId = showingId,
            SeatNumber = seat,
            TicketType = "Regular",
            Price = 15.00m,
            Status = "Active",
            PaymentStatus = "Pending",
            ShowDateTimeUtc = "2026-01-01T20:00:00.0000000+00:00",
            RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+00:00"
        };
    }

    // -------------------------------------------------------
    // GetAllAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_ReturnsAllTickets()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetAllAsync_ReturnsAllTickets));

        var movie = BuildMovie();
        var auditorium = BuildAuditorium();
        var showing = BuildShowing(1, movie.Id, auditorium.Id);

        db.Movies.Add(movie);
        db.Auditoriums.Add(auditorium);
        db.Showings.Add(showing);
        db.Tickets.AddRange(
            BuildTicket(1, showing.Id),
            BuildTicket(2, showing.Id, "A2")
        );
        await db.SaveChangesAsync();

        var repo = new TicketRepository(db);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.NotNull(t.Showing));
    }

    // -------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsTicket()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetByIdAsync_WhenExists_ReturnsTicket));

        var movie = BuildMovie(2);
        var auditorium = BuildAuditorium(2);
        var showing = BuildShowing(2, movie.Id, auditorium.Id);
        var ticket = BuildTicket(10, showing.Id);

        db.Movies.Add(movie);
        db.Auditoriums.Add(auditorium);
        db.Showings.Add(showing);
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();

        var repo = new TicketRepository(db);

        var result = await repo.GetByIdAsync(ticket.Id);

        Assert.NotNull(result);
        Assert.Equal(ticket.Id, result.Id);
        Assert.NotNull(result.Showing);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetByIdAsync_WhenNotExists_ReturnsNull));
        var repo = new TicketRepository(db);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    // -------------------------------------------------------
    // GetTicketsByShowingIdAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetTicketsByShowingIdAsync_WhenFound_ReturnsFilteredTickets()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetTicketsByShowingIdAsync_WhenFound_ReturnsFilteredTickets));

        var movie = BuildMovie(3);
        var auditorium = BuildAuditorium(3);
        var showing1 = BuildShowing(3, movie.Id, auditorium.Id);
        var showing2 = BuildShowing(4, movie.Id, auditorium.Id);

        db.Movies.Add(movie);
        db.Auditoriums.Add(auditorium);
        db.Showings.AddRange(showing1, showing2);

        db.Tickets.AddRange(
            BuildTicket(21, showing1.Id, "B1"),
            BuildTicket(22, showing1.Id, "B2"),
            BuildTicket(23, showing2.Id, "C1")
        );
        await db.SaveChangesAsync();

        var repo = new TicketRepository(db);

        var result = await repo.GetTicketsByShowingIdAsync(showing1.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(showing1.Id, t.ShowingId));
        Assert.All(result, t => Assert.NotNull(t.Showing?.Movie));
    }

    [Fact]
    public async Task GetTicketsByShowingIdAsync_WhenNotFound_ReturnsEmptyList()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetTicketsByShowingIdAsync_WhenNotFound_ReturnsEmptyList));
        var repo = new TicketRepository(db);

        var result = await repo.GetTicketsByShowingIdAsync(777);

        Assert.Empty(result);
    }

    // -------------------------------------------------------
    // AddAsync
    // -------------------------------------------------------

    [Fact]
    public async Task AddAsync_SavesTicket()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(AddAsync_SavesTicket));
        var repo = new TicketRepository(db);

        var ticket = BuildTicket(31, 55);

        await repo.AddAsync(ticket);

        Assert.Single(db.Tickets);
    }

    // -------------------------------------------------------
    // UpdateAsync
    // -------------------------------------------------------

    [Fact]
    public async Task UpdateAsync_UpdatesTicket()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(UpdateAsync_UpdatesTicket));

        var ticket = BuildTicket(41, 60);
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();

        var repo = new TicketRepository(db);

        ticket.Status = "Used";
        ticket.PaymentStatus = "Paid";

        await repo.UpdateAsync(ticket);

        var updated = db.Tickets.Single();
        Assert.Equal("Used", updated.Status);
        Assert.Equal("Paid", updated.PaymentStatus);
    }

    // -------------------------------------------------------
    // DeleteAsync
    // -------------------------------------------------------

    [Fact]
    public async Task DeleteAsync_WhenExists_RemovesTicket()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(DeleteAsync_WhenExists_RemovesTicket));

        var ticket = BuildTicket(51, 70);
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();

        var repo = new TicketRepository(db);

        await repo.DeleteAsync(ticket.Id);

        Assert.Empty(db.Tickets);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_DoesNothing()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(DeleteAsync_WhenNotExists_DoesNothing));

        var ticket = BuildTicket(61, 80);
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();

        var repo = new TicketRepository(db);

        await repo.DeleteAsync(999);

        Assert.Single(db.Tickets);
    }
}




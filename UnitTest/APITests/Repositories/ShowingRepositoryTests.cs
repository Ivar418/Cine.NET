using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using Xunit;
using UnitTest.Helpers;

namespace UnitTest.APITests.Repositories;

public class ShowingRepositoryTests
{
    private static Auditorium BuildAuditorium()
    {
        var a = new Auditorium { Id = 1, Name = "Room 1" };
        a.SetRows(new List<RowConfig> { new(10, 0) });
        return a;
    }

    private static Movie BuildMovie()
    {
        return new Movie
        {
            Id = 1,
            Title = "Test",
            TmdbId = 1,
            RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+00:00"
        };
    }

    [Fact]
    public async Task AddShowingAsync_CreatesShowingWithSnapshot()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(AddShowingAsync_CreatesShowingWithSnapshot));

        var auditorium = BuildAuditorium();
        var movie = BuildMovie();

        db.Auditoriums.Add(auditorium);
        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        IShowingRepository repo = new ShowingRepository(db);

        var request = new CreateShowingRequest(movie.Id, auditorium.Id, DateTimeOffset.UtcNow, false);

        var result = await repo.AddShowingAsync(request);

        Assert.NotNull(result);
        Assert.NotEqual("[]", result.AuditoriumLayoutSnapshot);
    }

    [Fact]
    public async Task GetShowingAsync_WhenExists_ReturnsShowing()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetShowingAsync_WhenExists_ReturnsShowing));

        var movie = BuildMovie();
        var auditorium = BuildAuditorium();

        var showing = new Showing
        {
            Movie = movie,
            Auditorium = auditorium
        };

        db.Showings.Add(showing);
        await db.SaveChangesAsync();

        IShowingRepository repo = new ShowingRepository(db);

        var result = await repo.GetShowingAsync(showing.Id);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetShowingAsync_WhenNotExists_ReturnsFailure()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetShowingAsync_WhenNotExists_ReturnsFailure));
        IShowingRepository repo = new ShowingRepository(db);

        var result = await repo.GetShowingAsync(999);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task GetUpcomingShowingsByMovieIdAsync_ReturnsFiltered()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetUpcomingShowingsByMovieIdAsync_ReturnsFiltered));

        var movie = BuildMovie();
        db.Movies.Add(movie);

        var auditorium = BuildAuditorium();
        db.Auditoriums.Add(auditorium);

        db.Showings.Add(new Showing
        {
            MovieId = movie.Id,
            AuditoriumId = auditorium.Id,
            StartsAt = DateTimeOffset.UtcNow.AddHours(2)
        });

        await db.SaveChangesAsync();

        IShowingRepository repo = new ShowingRepository(db);

        var result = await repo.GetUpcomingShowingsByMovieIdAsync(movie.Id, DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }
}
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using Xunit;
using UnitTest.Helpers;

namespace UnitTest.APITests.Repositories;

public class AuditoriumRepositoryTests
{
    [Fact]
    public async Task AddAuditoriumAsync_SavesAuditorium()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(AddAuditoriumAsync_SavesAuditorium));
        IAuditoriumRepository repo = new AuditoriumRepository(db);

        var request = new CreateAuditoriumRequest(
            "Zaal 7",
            new List<RowConfig>
            {
                new(10, 0)
            }
        );

        var result = await repo.AddAuditoriumAsync(request);

        Assert.NotNull(result);
        Assert.Single(db.Auditoriums);
    }

    [Fact]
    public async Task GetAuditoriumAsync_WhenExists_ReturnsSuccess()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetAuditoriumAsync_WhenExists_ReturnsSuccess));

        db.Auditoriums.Add(new Auditorium { Name = "Test" });
        await db.SaveChangesAsync();

        IAuditoriumRepository repo = new AuditoriumRepository(db);

        var result = await repo.GetAuditoriumAsync(1);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAuditoriumAsync_WhenNotExists_ReturnsFailure()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetAuditoriumAsync_WhenNotExists_ReturnsFailure));
        IAuditoriumRepository repo = new AuditoriumRepository(db);

        var result = await repo.GetAuditoriumAsync(999);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task GetAuditoriumsAsync_ReturnsAll()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetAuditoriumsAsync_ReturnsAll));

        db.Auditoriums.Add(new Auditorium { Name = "Zaal 7" });
        db.Auditoriums.Add(new Auditorium { Name = "Zaal 8" });
        await db.SaveChangesAsync();

        IAuditoriumRepository repo = new AuditoriumRepository(db);

        var result = await repo.GetAuditoriumsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
    }
}
using API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Helpers;

public static class TestDbContextFactory
{
    public static ApiDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApiDbContext(options);
    }
}
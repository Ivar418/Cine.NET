using API.Repositories.Implementations;
using SharedLibrary.Domain.Entities;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.APITests.Repositories;

public class OrderRepositoryTests
{
    private static Ticket BuildTicket(int id = 1, int showingId = 1)
    {
        return new Ticket
        {
            Id = id,
            ShowingId = showingId,
            SeatNumber = "A1",
            TicketType = "Regular",
            Price = 12.50m,
            ShowDateTimeUtc = "2026-01-01T20:00:00.0000000+00:00",
            RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+00:00"
        };
    }

    private static Order BuildOrder(int id = 1)
    {
        return new Order
        {
            Id = id,
            OrderCode = $"ORD-{id:000}",
            TotalAmount = 12.50m
        };
    }

    // -------------------------------------------------------
    // AddAsync
    // -------------------------------------------------------

    [Fact]
    public async Task AddAsync_SavesOrder()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(AddAsync_SavesOrder));
        var repo = new OrderRepository(db);

        var order = BuildOrder();

        await repo.AddAsync(order);

        Assert.Single(db.Orders);
    }

    // -------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsOrder()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetByIdAsync_WhenExists_ReturnsOrder));

        var order = BuildOrder();
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var repo = new OrderRepository(db);

        var result = await repo.GetByIdAsync(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetByIdAsync_WhenNotExists_ReturnsNull));
        var repo = new OrderRepository(db);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    // -------------------------------------------------------
    // GetByIdWithTicketsAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetByIdWithTicketsAsync_WhenExists_ReturnsOrderWithTickets()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetByIdWithTicketsAsync_WhenExists_ReturnsOrderWithTickets));

        var order = BuildOrder();
        var ticket = BuildTicket();

        db.Orders.Add(order);
        db.Tickets.Add(ticket);
        db.OrderTickets.Add(new OrderTicket
        {
            OrderId = order.Id,
            TicketId = ticket.Id
        });
        await db.SaveChangesAsync();

        var repo = new OrderRepository(db);

        var result = await repo.GetByIdWithTicketsAsync(order.Id);

        Assert.NotNull(result);
        Assert.Single(result.OrderTickets);
        Assert.Equal(ticket.Id, result.OrderTickets.First().TicketId);
        Assert.NotNull(result.OrderTickets.First().Ticket);
    }

    [Fact]
    public async Task GetByIdWithTicketsAsync_WhenNotExists_ReturnsNull()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(GetByIdWithTicketsAsync_WhenNotExists_ReturnsNull));
        var repo = new OrderRepository(db);

        var result = await repo.GetByIdWithTicketsAsync(123);

        Assert.Null(result);
    }

    // -------------------------------------------------------
    // SaveChangesAsync
    // -------------------------------------------------------

    [Fact]
    public async Task SaveChangesAsync_PersistsTrackedChanges()
    {
        var db = TestDbContextFactory.CreateDbContext(nameof(SaveChangesAsync_PersistsTrackedChanges));

        var order = BuildOrder();
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var repo = new OrderRepository(db);

        order.PaymentStatus = "Paid";
        await repo.SaveChangesAsync();

        Assert.Equal("Paid", db.Orders.Single().PaymentStatus);
    }
}



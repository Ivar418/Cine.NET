using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly ApiDbContext _db;

    public OrderRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Persists a new order and commits changes.
    /// </summary>
    /// <param name="order">The order entity to store.</param>
    public async Task AddAsync(Order order)
    {
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
    }
    
    /// <summary>
    /// Retrieves an order by identifier including linked tickets.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>The matching order with ticket graph, or <c>null</c> when not found.</returns>
    public async Task<Order?> GetByIdWithTicketsAsync(int orderId)
    {
        return await _db.Orders
            .Include(o => o.OrderTickets)
            .ThenInclude(ot => ot.Ticket)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    /// <summary>
    /// Retrieves an order by identifier without eager-loading ticket relations.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>The matching order, or <c>null</c> when not found.</returns>
    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
    
    /// <summary>
    /// Retrieves all orders including tickets, ordered by creation date descending.
    /// </summary>
    /// <returns>A list of orders with ticket relations.</returns>
    public async Task<List<Order>> GetAllWithTicketsAsync()
    {
        return await _db.Orders
            .Include(o => o.OrderTickets)
            .ThenInclude(ot => ot.Ticket)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync();
    }

    /// <summary>
    /// Commits pending data changes to the database.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}

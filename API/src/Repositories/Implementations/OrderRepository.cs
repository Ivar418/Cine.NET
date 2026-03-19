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

    public async Task AddAsync(Order order)
    {
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
    }
    
    public async Task<Order?> GetByIdWithTicketsAsync(int orderId)
    {
        return await _db.Orders
            .Include(o => o.OrderTickets)
            .ThenInclude(ot => ot.Ticket)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}


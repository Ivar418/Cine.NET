using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdWithTicketsAsync(int orderId);
    Task SaveChangesAsync();
}

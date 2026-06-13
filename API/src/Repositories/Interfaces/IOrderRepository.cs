using API.Domain.Common;
using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(int orderId);
    Task<Order?> GetByIdWithTicketsAsync(int orderId);
    Task<List<Order>> GetAllWithTicketsAsync();
    Task SaveChangesAsync();
    Task<ResultOf<List<Order>>> GetAllOrdersByUserId(int userId);
}

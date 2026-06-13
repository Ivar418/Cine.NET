using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;


namespace API.Services.Implementations;

public class TicketService : ITicketService {
    private readonly ITicketRepository _repository;
    private readonly IOrderRepository _orderRepository;

    public TicketService(ITicketRepository repository, IOrderRepository orderRepository) {
        _repository = repository;
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<Ticket>> GetAllTicketsAsync()
        => await _repository.GetAllAsync();

    public async Task<Ticket?> GetTicketByIdAsync(int id)
        => await _repository.GetByIdAsync(id);


    public async Task<IReadOnlyList<Ticket>> GetShowingTicketsAsync(int showingId)
        => await _repository.GetTicketsByShowingIdAsync(showingId);

    public async Task<Ticket> CreateTicketAsync(Ticket ticket) {
        await _repository.AddAsync(ticket);
        return ticket;
    }

    public async Task UpdateTicketAsync(Ticket ticket)
        => await _repository.UpdateAsync(ticket);

    public async Task DeleteTicketAsync(int id)
        => await _repository.DeleteAsync(id);

    public async Task<ResultOf<List<Ticket>>> GetTicketsByOrderIdAsync(int orderId, int currentUserId) {
        var order = await _orderRepository.GetByIdAsync(orderId);
        var tickets = await _repository.GetTicketsByOrderIdAsync(orderId);
        return order?.UserId != currentUserId ? ResultOf<List<Ticket>>.Failure("Unauthorized access to order tickets.") : tickets;
    }
}
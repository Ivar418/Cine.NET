using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces;

public interface ITicketTypeRepository
{
    Task<List<TicketType>> GetAllAsync();
}
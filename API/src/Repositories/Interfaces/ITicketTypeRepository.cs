using API.Domain.Common;
using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces;

public interface ITicketTypeRepository
{
    Task<ResultOf<List<TicketType>>> GetAllAsync();
}
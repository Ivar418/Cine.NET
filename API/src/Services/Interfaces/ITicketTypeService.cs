using API.Domain.Common;
using SharedLibrary.Domain.Entities;

namespace API.Services.Interfaces;

public interface ITicketTypeService
{
    Task<ResultOf<List<TicketType>>> GetAllAsync();
}
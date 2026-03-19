using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

public class TicketTypeService : ITicketTypeService
{
    private readonly ITicketTypeRepository _repository;

    public TicketTypeService(ITicketTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultOf<List<TicketType>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();

        if (result.IsFailure)
            return ResultOf<List<TicketType>>.Failure(result.Error!);

        return result;
    }
}
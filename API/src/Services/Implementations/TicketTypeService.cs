using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

/// <summary>
/// Service responsible for retrieving TicketType data.
/// </summary>
public class TicketTypeService : ITicketTypeService
{
    private readonly ITicketTypeRepository _repository;

    /// <summary>
    /// Initializes a new instance of the TicketTypeService.
    /// </summary>
    /// <param name="repository">The TicketType repository.</param>
    public TicketTypeService(ITicketTypeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves all TicketTypes.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a list of <see cref="TicketType"/> on success.
    /// Returns a failure result if the repository call fails.
    /// </returns>
    public async Task<ResultOf<List<TicketType>>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();

        if (result.IsFailure)
            return ResultOf<List<TicketType>>.Failure(result.Error!);

        return result;
    }
}
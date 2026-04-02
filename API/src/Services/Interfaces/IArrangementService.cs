using API.Domain.Common;
using SharedLibrary.Domain.Entities;

/// <summary>
/// Defines business logic operations for arrangements.
/// </summary>
namespace API.Services.Interfaces
{
    public interface IArrangementService
    {
        Task<ResultOf<IReadOnlyList<Arrangement>>> GetAllAsync();
        Task<ResultOf<Arrangement?>> GetByIdAsync(int id);
        Task<ResultOf<Arrangement>> CreateAsync(Arrangement arrangement);
    }
}
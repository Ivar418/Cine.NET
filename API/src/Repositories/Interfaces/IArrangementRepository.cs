using API.Domain.Common;
using SharedLibrary.Domain.Entities;

/// <summary>
/// Defines data access operations for arrangements.
/// </summary>
namespace API.Repositories.Interfaces
{
    public interface IArrangementRepository
    {
        Task<ResultOf<IReadOnlyList<Arrangement>>> GetAllAsync();
        Task<ResultOf<Arrangement?>> GetByIdAsync(int id);
        Task<ResultOf<Arrangement>> CreateAsync(Arrangement arrangement);
    }
}
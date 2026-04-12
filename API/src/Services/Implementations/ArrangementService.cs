using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

/// <summary>
/// Handles arrangement-related business logic.
/// </summary>
namespace API.Services.Implementations
{
    public class ArrangementService : IArrangementService
    {
        private readonly IArrangementRepository _repository;

        public ArrangementService(IArrangementRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultOf<IReadOnlyList<Arrangement>>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ResultOf<Arrangement?>> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ResultOf<Arrangement>> CreateAsync(Arrangement arrangement)
        {
            return await _repository.CreateAsync(arrangement);
        }
    }
}
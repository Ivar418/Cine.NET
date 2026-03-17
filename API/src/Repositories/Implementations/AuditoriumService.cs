using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.Repositories.Implementations;

public class AuditoriumService : IAuditoriumService
{
    private readonly IAuditoriumRepository _auditoriumRepository;

    public AuditoriumService(IAuditoriumRepository auditoriumRepository)
    {
        _auditoriumRepository = auditoriumRepository;
    }

    public async Task<ResultOf<Auditorium>> GetAuditoriumAsync(int id)
    {
        var result = await _auditoriumRepository.GetAuditoriumAsync(id);
        return result;
    }

    public async Task<ResultOf<ICollection<Auditorium>>> GetAuditoriumsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Auditorium> AddAuditoriumAsync(CreateAuditoriumRequest auditorium)
    {
        throw new NotImplementedException();
    }

    public async Task<Auditorium> UpdateAuditoriumAsync(Auditorium auditorium)
    {
        throw new NotImplementedException();
    }

    public async Task<ResultOf<Auditorium>> DeleteAuditoriumByIdAsync(int auditoriumId)
    {
        throw new NotImplementedException();
    }
}
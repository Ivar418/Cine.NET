using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Repositories.Interfaces
{
    public interface IAuditoriumRepository
    {
        Task<ResultOf<Auditorium>> GetAuditoriumAsync(int id);
        Task<ResultOf<ICollection<Auditorium>>> GetAuditoriumsAsync();
        Task<Auditorium> AddAuditoriumAsync(CreateAuditoriumRequest Auditorium);
        Task<Auditorium> UpdateAuditoriumAsync(Auditorium Auditorium);
        Task<ResultOf<Auditorium>> DeleteAuditoriumByIdAsync(int AuditoriumId);

    }
}

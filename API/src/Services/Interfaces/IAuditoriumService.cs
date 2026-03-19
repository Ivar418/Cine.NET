using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.Services.Interfaces;

public interface IAuditoriumService
{
    Task<ResultOf<Auditorium>> GetAuditoriumAsync(int id);
    Task<ResultOf<ICollection<Auditorium>>> GetAuditoriumsAsync();
    Task<Auditorium> AddAuditoriumAsync(CreateAuditoriumRequest auditorium);
    Task<Auditorium> UpdateAuditoriumAsync(Auditorium auditorium);
    Task<ResultOf<Auditorium>> DeleteAuditoriumByIdAsync(int auditoriumId);
}
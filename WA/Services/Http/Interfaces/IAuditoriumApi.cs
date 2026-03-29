using SharedLibrary.Domain.Entities;

namespace WA.Services.Http.Interfaces;

public interface IAuditoriumApi
{
    Task<List<Auditorium>> GetAllAuditoriumsAsync();
    Task<Auditorium?> GetAuditoriumByIdAsync(int id);
}

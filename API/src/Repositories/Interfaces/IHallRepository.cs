using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.src.Repositories.Interfaces
{
    public interface IHallRepository
    {
        Task<ResultOf<Hall>> GetHallAsync(int id);
        Task<ResultOf<ICollection<Hall>>> GetHallsAsync();
        Task<Hall> AddHallAsync(CreateHallRequest hall);
        Task<Hall> UpdateHallAsync(Hall hall);
        Task<ResultOf<Hall>> DeleteHallByIdAsync(int hallId);

    }
}

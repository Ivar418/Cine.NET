using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.Repositories.Implementations
{
    public class AuditoriumRepository : IAuditoriumRepository
    {
        private readonly ApiDbContext _db;

        public AuditoriumRepository(ApiDbContext db)
        {
            _db = db;
        }

        async Task<Auditorium> IAuditoriumRepository.AddAuditoriumAsync(CreateAuditoriumRequest Auditorium)
        {
            Console.WriteLine($"Adding Auditorium: {Auditorium.Name}");
            Auditorium newAuditorium = new Auditorium
            {
                Name = Auditorium.Name
            };
            newAuditorium.SetRows(Auditorium.Rows);

            var result = await _db.Auditoriums.AddAsync(newAuditorium);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        async Task<ResultOf<Auditorium>> IAuditoriumRepository.DeleteAuditoriumByIdAsync(int AuditoriumId)
        {
            throw new NotImplementedException();
        }

        async Task<ResultOf<Auditorium>> IAuditoriumRepository.GetAuditoriumAsync(int id)
        {
            var Auditorium = await _db.Auditoriums.FindAsync(id);
            return Auditorium == null ? ResultOf<Auditorium>.Failure("Auditorium not found") : ResultOf<Auditorium>.Success(Auditorium);
        }

        async Task<ResultOf<ICollection<Auditorium>>> IAuditoriumRepository.GetAuditoriumsAsync()
        {
            try
            {
                var Auditoriums = await _db.Auditoriums.ToListAsync();

                return ResultOf<ICollection<Auditorium>>.Success(Auditoriums);
            }
            catch (Exception e)
            {
                return ResultOf<ICollection<Auditorium>>.Failure(e.Message);
            }
        }

        async Task<Auditorium> IAuditoriumRepository.UpdateAuditoriumAsync(Auditorium Auditorium)
        {
            throw new NotImplementedException();
        }
    }
}

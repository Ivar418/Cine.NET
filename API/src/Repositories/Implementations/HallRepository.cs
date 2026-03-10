using System.Net.Http.Headers;
using System.Text.Json;
using API.Domain.Common;
using API.Infrastructure.Database;
using API.src.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using Microsoft.EntityFrameworkCore;

namespace API.src.Repositories.Implementations
{
    public class HallRepository : IHallRepository
    {
        private readonly ApiDbContext _db;

        public HallRepository(ApiDbContext db)
        {
            _db = db;
        }

        async Task<Hall> IHallRepository.AddHallAsync(CreateHallRequest hall)
        {
            Console.WriteLine($"Adding hall: {hall.Name}");
            Hall newHall = new Hall
            {
                Name = hall.Name
            };
            newHall.SetRows(hall.Rows);

            var result = await _db.Halls.AddAsync(newHall);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        async Task<ResultOf<Hall>> IHallRepository.DeleteHallByIdAsync(int hallId)
        {
            throw new NotImplementedException();
        }

        async Task<ResultOf<Hall>> IHallRepository.GetHallAsync(int id)
        {
            var hall = await _db.Halls.FindAsync(id);
            return hall == null ? ResultOf<Hall>.Failure("Hall not found") : ResultOf<Hall>.Success(hall);
        }

        async Task<ResultOf<ICollection<Hall>>> IHallRepository.GetHallsAsync()
        {
            try
            {
                var halls = await _db.Halls.ToListAsync();

                return ResultOf<ICollection<Hall>>.Success(halls);
            }
            catch (Exception e)
            {
                return ResultOf<ICollection<Hall>>.Failure(e.Message);
            }
        }

        async Task<Hall> IHallRepository.UpdateHallAsync(Hall hall)
        {
            throw new NotImplementedException();
        }
    }
}

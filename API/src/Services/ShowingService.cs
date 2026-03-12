using Microsoft.EntityFrameworkCore;
using API.Infrastructure.Database;
using API.Services.Implementations;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Services;

public class ShowingService
{
    private readonly ApiDbContext _db;
    private readonly PricingService _pricingService;

    public ShowingService(ApiDbContext db, PricingService pricingService)
    {
        _db = db;
        _pricingService = pricingService;
    }

    public async Task<List<ShowingsWithPricesResponse>> GetShowingsAsync()
    {
        var showings = await _db.Showings
            .Include(s => s.Movie)
            .Include(s => s.Auditorium)
            .ToListAsync();

        var ticketTypes = await _db.TicketTypes.ToListAsync();

        var adult = ticketTypes.First(t => t.Name == "Adult");
        var child = ticketTypes.First(t => t.Name == "Child");
        var student = ticketTypes.First(t => t.Name == "Student");
        var senior = ticketTypes.First(t => t.Name == "Senior");

        return showings.Select(s => new ShowingsWithPricesResponse
        {
            ShowingId = s.Id,
            MovieTitle = s.Movie.Title,
            Runtime = s.Movie.Runtime,
            AuditoriumId = s.AuditoriumId,
            AuditoriumName = s.Auditorium.Name,
            StartsAt = s.StartsAt,

            Prices = new ShowingPricesResponse
            {
                Adult = _pricingService.CalculatePrice(s.Movie, s.IsThreeD, adult),
                Child = _pricingService.CalculatePrice(s.Movie, s.IsThreeD, child),
                Student = _pricingService.CalculatePrice(s.Movie, s.IsThreeD, student),
                Senior = _pricingService.CalculatePrice(s.Movie, s.IsThreeD, senior)
            }

        }).ToList();
    }
}
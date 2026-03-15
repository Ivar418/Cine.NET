using API.Infrastructure.Database;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class ShowingService : IShowingService
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
            .ToListAsync();
        
        var (adult, child, student, senior) = await GetTicketTypes();

        return showings.Select(s => new ShowingsWithPricesResponse
        {
            ShowingId = s.Id,
            MovieTitle = s.Movie.Title,
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
    
    public async Task<ShowingsWithPricesResponse?> GetShowingAsync(int id)
    {
        var showing = await _db.Showings
            .Include(s => s.Movie)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (showing == null)
            return null;
        
        var (adult, child, student, senior) = await GetTicketTypes();

        return new ShowingsWithPricesResponse
        {
            ShowingId = showing.Id,
            MovieTitle = showing.Movie.Title,
            StartsAt = showing.StartsAt,
            Prices = new ShowingPricesResponse
            {
                Adult = _pricingService.CalculatePrice(showing.Movie, showing.IsThreeD, adult),
                Child = _pricingService.CalculatePrice(showing.Movie, showing.IsThreeD, child),
                Student = _pricingService.CalculatePrice(showing.Movie, showing.IsThreeD, student),
                Senior = _pricingService.CalculatePrice(showing.Movie, showing.IsThreeD, senior)
            }
        };
    }
    
    private async Task<(TicketType adult, TicketType child, TicketType student, TicketType senior)> GetTicketTypes()
    {
        var ticketTypes = await _db.TicketTypes.ToListAsync();

        return (
            ticketTypes.First(t => t.Name == "Adult"),
            ticketTypes.First(t => t.Name == "Child"),
            ticketTypes.First(t => t.Name == "Student"),
            ticketTypes.First(t => t.Name == "Senior")
        );
    }
}
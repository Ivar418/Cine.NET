using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.src.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class ShowingService : IShowingService
{
    private readonly IShowingRepository _showingRepository;
    private readonly IPricingService _pricingService;
    private readonly ITicketTypeRepository _ticketTypeRepository;

    public ShowingService(
        IShowingRepository repository,
        IPricingService pricingService,
        ITicketTypeRepository ticketTypeRepository)
    {
        _showingRepository = repository;
        _pricingService = pricingService;
        _ticketTypeRepository = ticketTypeRepository;
    }

    public async Task<ResultOf<List<ShowingsWithPricesResponse>>> GetShowingsAsync()
    {
        var showingsResult = await _showingRepository.GetShowingsAsync();

        if (showingsResult.IsFailure)
            return ResultOf<List<ShowingsWithPricesResponse>>.Failure(showingsResult.Error!);

        var showings = showingsResult.Value!;

        var (adult, child, student, senior) = await GetTicketTypes();

        var result = showings.Select(s => new ShowingsWithPricesResponse
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

        return ResultOf<List<ShowingsWithPricesResponse>>.Success(result);
    }
    
    public async Task<ResultOf<ShowingsWithPricesResponse>> GetShowingAsync(int id)
    {
        var showingResult = await _showingRepository.GetShowingAsync(id);

        if (showingResult.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(showingResult.Error!);

        var showing = showingResult.Value!;

        var (adult, child, student, senior) = await GetTicketTypes();

        var response = new ShowingsWithPricesResponse
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

        return ResultOf<ShowingsWithPricesResponse>.Success(response);
    }
    
    private async Task<(TicketType adult, TicketType child, TicketType student, TicketType senior)> GetTicketTypes()
    {
        var ticketTypes = await _ticketTypeRepository.GetAllAsync();

        return (
            ticketTypes.First(t => t.Name == "Adult"),
            ticketTypes.First(t => t.Name == "Child"),
            ticketTypes.First(t => t.Name == "Student"),
            ticketTypes.First(t => t.Name == "Senior")
        );
    }
}
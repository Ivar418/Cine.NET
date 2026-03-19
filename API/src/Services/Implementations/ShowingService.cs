using System.Runtime.InteropServices.JavaScript;
using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class ShowingService : IShowingService
{
    private readonly IShowingRepository _showingRepository;
    private readonly IPricingService _pricingService;
    private readonly ITicketTypeService _ticketTypeService;

    public ShowingService(
        IShowingRepository repository,
        IPricingService pricingService,
        ITicketTypeService ticketTypeService)
    {
        _showingRepository = repository;
        _pricingService = pricingService;
        _ticketTypeService = ticketTypeService;
    }

    // /{id}/prices 
    public async Task<ResultOf<ShowingsWithPricesResponse>> GetShowingAsync(int id)
    {
        var showingResult = await _showingRepository.GetShowingAsync(id);

        if (showingResult.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(showingResult.Error!);

        var showing = showingResult.Value;

        if (showing == null)
            return ResultOf<ShowingsWithPricesResponse>.Failure("NotFound");

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
    
    // /prices
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
    
    private async Task<ResultOf<(TicketType adult, TicketType child, TicketType student, TicketType senior)>> GetTicketTypes()
    {
        var result = await _ticketTypeService.GetAllAsync();

        if (result.IsFailure)
            return ResultOf<(TicketType, TicketType, TicketType, TicketType)>.Failure(result.Error!);

        var ticketTypes = result.Value!;

        try
        {
            return ResultOf<(TicketType, TicketType, TicketType, TicketType)>.Success((
                ticketTypes.First(t => t.Name == "Adult"),
                ticketTypes.First(t => t.Name == "Child"),
                ticketTypes.First(t => t.Name == "Student"),
                ticketTypes.First(t => t.Name == "Senior")
            ));
        }
        catch
        {
            return ResultOf<(TicketType, TicketType, TicketType, TicketType)>.Failure("TicketTypes not configured correctly");
        }
    }
}
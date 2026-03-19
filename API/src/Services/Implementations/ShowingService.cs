using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class ShowingService : IShowingService
{
    private readonly IShowingRepository _showingRepository;
    private readonly IPricingService _pricingService;
    private readonly ITicketTypeService _ticketTypeService;
    private readonly ITicketRuleService _ticketRuleService;

    public ShowingService(
        IShowingRepository repository,
        IPricingService pricingService,
        ITicketTypeService ticketTypeService,
        ITicketRuleService ticketRuleService)
    {
        _showingRepository = repository;
        _pricingService = pricingService;
        _ticketTypeService = ticketTypeService;
        _ticketRuleService = ticketRuleService;
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

        var ticketTypesResult = await GetTicketTypes();

        if (ticketTypesResult.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(ticketTypesResult.Error!);

        var (adult, child, student, senior) = ticketTypesResult.Value;

        return await BuildShowingResponseAsync(showing, adult, child, student, senior);
    }

    // /prices
    public async Task<ResultOf<List<ShowingsWithPricesResponse>>> GetShowingsAsync()
    {
        var showingsResult = await _showingRepository.GetShowingsAsync();

        if (showingsResult.IsFailure)
            return ResultOf<List<ShowingsWithPricesResponse>>.Failure(showingsResult.Error!);

        var showings = showingsResult.Value!;

        var ticketTypesResult = await GetTicketTypes();

        if (ticketTypesResult.IsFailure)
            return ResultOf<List<ShowingsWithPricesResponse>>.Failure(ticketTypesResult.Error!);

        var (adult, child, student, senior) = ticketTypesResult.Value!;

        var result = new List<ShowingsWithPricesResponse>();

        foreach (var s in showings)
        {
            var responseResult = await BuildShowingResponseAsync(s, adult, child, student, senior);

            if (responseResult.IsFailure)
                return ResultOf<List<ShowingsWithPricesResponse>>.Failure(responseResult.Error!);

            result.Add(responseResult.Value!);
        }

        return ResultOf<List<ShowingsWithPricesResponse>>.Success(result);
    }
    
    // Helpder: haalt alle TicketTypes op en mapt deze naar de vier verwachte categorieën
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
    
    // Helper: bouwt een Showing response inclusief prijsberekeningen per tickettype.
    private async Task<ResultOf<ShowingsWithPricesResponse>> BuildShowingResponseAsync(
        Showing showing,
        TicketType adult,
        TicketType child,
        TicketType student,
        TicketType senior)
    {
        var now = DateTime.Now;

        async Task<TicketPriceResponse> Build(TicketType type)
        {
            var priceResult = await _pricingService.CalculatePriceAsync(
                showing.Movie,
                showing.IsThreeD,
                type);

            if (priceResult.IsFailure)
                throw new Exception(priceResult.Error);

            var isAvailable = _ticketRuleService.IsTicketTypeAvailable(type, showing, now);

            return new TicketPriceResponse
            {
                Price = priceResult.Value!,
                IsAvailable = isAvailable
            };
        }

        try
        {
            return ResultOf<ShowingsWithPricesResponse>.Success(new ShowingsWithPricesResponse
            {
                ShowingId = showing.Id,
                MovieTitle = showing.Movie.Title,
                StartsAt = showing.StartsAt,
                Prices = new ShowingPricesResponse
                {
                    Adult = await Build(adult),
                    Child = await Build(child),
                    Student = await Build(student),
                    Senior = await Build(senior)
                }
            });
        }
        catch (Exception ex)
        {
            return ResultOf<ShowingsWithPricesResponse>.Failure(ex.Message);
        }
    }
}
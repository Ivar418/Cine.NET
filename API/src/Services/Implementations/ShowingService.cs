using API.Domain.Common;
using API.Services.Interfaces;
using API.src.Repositories.Interfaces;
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

        var ticketTypesResult = await GetTicketTypes();

        if (ticketTypesResult.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(ticketTypesResult.Error!);

        var (adult, child, student, senior) = ticketTypesResult.Value!;

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
        var adultPrice = await _pricingService.CalculatePriceAsync(showing.Movie, showing.IsThreeD, adult);
        if (adultPrice.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(adultPrice.Error!);

        var childPrice = await _pricingService.CalculatePriceAsync(showing.Movie, showing.IsThreeD, child);
        if (childPrice.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(childPrice.Error!);

        var studentPrice = await _pricingService.CalculatePriceAsync(showing.Movie, showing.IsThreeD, student);
        if (studentPrice.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(studentPrice.Error!);

        var seniorPrice = await _pricingService.CalculatePriceAsync(showing.Movie, showing.IsThreeD, senior);
        if (seniorPrice.IsFailure)
            return ResultOf<ShowingsWithPricesResponse>.Failure(seniorPrice.Error!);

        return ResultOf<ShowingsWithPricesResponse>.Success(new ShowingsWithPricesResponse
        {
            ShowingId = showing.Id,
            MovieTitle = showing.Movie.Title,
            StartsAt = showing.StartsAt,
            Prices = new ShowingPricesResponse
            {
                Adult = adultPrice.Value!,
                Child = childPrice.Value!,
                Student = studentPrice.Value!,
                Senior = seniorPrice.Value!
            }
        });
    }
}
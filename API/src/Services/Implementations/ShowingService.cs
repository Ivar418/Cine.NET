using System.Collections;
using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Mappers;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

/// <summary>
/// Service responsible for retrieving Showings and composing responses with pricing and availability logic.
/// </summary>
public class ShowingService : IShowingService
{
    private readonly IShowingRepository _showingRepository;
    private readonly IPricingService _pricingService;
    private readonly ITicketTypeService _ticketTypeService;
    private readonly ITicketRuleService _ticketRuleService;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IReservationRepository _reservationrepository;

    /// <summary>
    /// Initializes a new instance of the ShowingService.
    /// </summary>
    public ShowingService(
        IShowingRepository repository,
        IPricingService pricingService,
        ITicketTypeService ticketTypeService,
        ITicketTypeRepository ticketTypeRepository,
        IReservationRepository reservationRepository,
        ITicketRuleService ticketRuleService)
    {
        _showingRepository = repository;
        _pricingService = pricingService;
        _ticketTypeService = ticketTypeService;
        _ticketTypeRepository = ticketTypeRepository;
        _reservationrepository = reservationRepository;
        _ticketRuleService = ticketRuleService;
    }

    /// <summary>
    /// Retrieves a Showing by ID including calculated prices per ticket type.
    /// </summary>
    /// <param name="id">The Showing ID.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a <see cref="ShowingsWithPricesResponse"/> on success.
    /// Returns a failure result if the Showing or required data cannot be retrieved.
    /// </returns>
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

    /// <summary>
    /// Retrieves the full Showing entity by ID without additional processing.
    /// </summary>
    /// <param name="id">The Showing ID.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the <see cref="Showing"/> on success.
    /// </returns>
    public async Task<ResultOf<Showing>> GetFullShowingByIdAsync(int id)
    {
        var showingsResult = await _showingRepository.GetShowingAsync(id);
        return showingsResult;
    }

    /// <summary>
    /// Retrieves all Showings and enriches them with pricing information.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a list of <see cref="ShowingsWithPricesResponse"/> on success.
    /// Returns a failure result if any step in the process fails.
    /// </returns>
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

    /// <summary>
    /// Retrieves all TicketTypes and maps them to the expected categories (Adult, Child, Student, Senior).
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a tuple of TicketTypes on success.
    /// Returns a failure result if TicketTypes are missing or misconfigured.
    /// </returns>
    private async Task<ResultOf<(TicketType adult, TicketType child, TicketType student, TicketType senior)>>
        GetTicketTypes()
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
            return ResultOf<(TicketType, TicketType, TicketType, TicketType)>.Failure(
                "TicketTypes not configured correctly");
        }
    }

    /// <summary>
    /// Builds a Showing response including calculated prices and availability per ticket type.
    /// </summary>
    /// <param name="showing">The Showing entity.</param>
    /// <param name="adult">The Adult ticket type.</param>
    /// <param name="child">The Child ticket type.</param>
    /// <param name="student">The Student ticket type.</param>
    /// <param name="senior">The Senior ticket type.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a <see cref="ShowingsWithPricesResponse"/> on success.
    /// Returns a failure result if price calculation fails.
    /// </returns>
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

    /// <summary>
    /// Retrieves upcoming Showings for a specific movie based on a cutoff time.
    /// </summary>
    /// <param name="movieId">The Movie ID.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a list of <see cref="ShowingResponse"/> on success.
    /// Returns a failure result if the repository query fails.
    /// </returns>
    public async Task<ResultOf<IReadOnlyList<ShowingResponse>>> GetUpcomingShowingsByMovieIdAsync(int movieId)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-15);

        var result = await _showingRepository.GetUpcomingShowingsByMovieIdAsync(movieId, cutoff);

        if (result.IsFailure)
            return ResultOf<IReadOnlyList<ShowingResponse>>.Failure(result.Error!);

        return ResultOf<IReadOnlyList<ShowingResponse>>.Success(result.Value!.ToList());
    }

    /// <summary>
    /// Retrieves the current state of a Showing including reservation data.
    /// </summary>
    /// <param name="id">The Showing ID.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a <see cref="ShowingStateDto"/> on success.
    /// Returns a failure result if the Showing or state cannot be determined.
    /// </returns>
    public async Task<ResultOf<ShowingStateDto>> GetShowingStateAsync(int id)
    {
        var showing = _showingRepository.GetShowingAsync(id).Result.Value;

        if (showing == null)
            return ResultOf<ShowingStateDto>.Failure("Showing not found");

        ShowingStateDto showingState = ShowingMapper.ToStateDto(showing, _reservationrepository);
        return showingState == null
            ? ResultOf<ShowingStateDto>.Failure("ShowingState not found")
            : ResultOf<ShowingStateDto>.Success(showingState);
    }

    /// <summary>
    /// Retrieves Showing display data optionally filtered by date.
    /// </summary>
    /// <param name="date">Optional date filter.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing a collection of <see cref="ShowingDisplayResponse"/> on success.
    /// </returns>
    public async Task<ResultOf<ICollection<ShowingDisplayResponse>>> GetShowingDisplayAsync(DateOnly? date = null)
    {
        return await _showingRepository.GetShowingDisplayAsync(date);
    }

    public async Task<ResultOf<Showing>> GetRandomShowingWithAmountOfSeatsAvailableAsync(int seatsNeededAmount)
    {
        var showings = await _showingRepository.GetShowingsAsync();
        if (showings.IsSuccess && showings.Value.Count > 0)
        {
            var upcomingAndSeatsAvailable = showings.Value.Where(s => s.StartsAt > DateTimeOffset.UtcNow);
            List<Showing> showingWithSeatsAvailable = new List<Showing>();
            foreach (var showing in upcomingAndSeatsAvailable)
            {
                var showingState = await GetShowingStateAsync(showing.Id);
                if (showingState.IsSuccess)
                {
                    var amountOfSeatsAvailable =
                        showingState.Value.AllSeats.Count - showingState.Value.OccupiedKeys.Count;
                    if (amountOfSeatsAvailable >= seatsNeededAmount) ;
                    {
                        showingWithSeatsAvailable.Add(showing);
                    }
                }
            }

            if (showingWithSeatsAvailable.Count > 0)
            {
                var random = new Random();
                int index = random.Next(showingWithSeatsAvailable.Count);
                return ResultOf<Showing>.Success(showingWithSeatsAvailable[index]);
            }
            return ResultOf<Showing>.Failure("No Showings with enough seats available");
        }
        return ResultOf<Showing>.Failure("An error occured");
    }
}
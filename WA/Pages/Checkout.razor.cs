using SharedLibrary.DTOs.Responses;

namespace WA.Pages;

using Microsoft.AspNetCore.Components;
using WA.Models;
using WA.Services.Http.Interfaces;

public partial class Checkout
{
    [Inject]
    public IShowingApi ShowingApi { get; set; } = default!;

    protected int step = 0;

    protected List<TicketSelection> seats = new()
    {
        new TicketSelection{ Row = 3, SeatNumber = 6 },
        new TicketSelection{ Row = 3, SeatNumber = 7 }
    };

    [Parameter]
    public int ShowingId { get; set; }

    protected ShowingsWithPricesResponse? showing;

    protected BookingSummary Summary => new()
    {
        MovieTitle = showing?.MovieTitle ?? string.Empty,
        StartsAt = showing?.StartsAt ?? default,
        Lines = seats,
        TotalPrice = seats.Sum(GetSeatPrice)
    };

    protected override async Task OnInitializedAsync()
    {
        showing = await ShowingApi.GetShowingPricesAsync(ShowingId);
    }

    protected decimal GetSeatPrice(TicketSelection seat)
    {
        if (showing is null || string.IsNullOrWhiteSpace(seat.TicketType))
            return 0m;

        return seat.TicketType switch
        {
            "Adult" => showing.Prices.Adult,
            "Student" => showing.Prices.Student,
            "Child" => showing.Prices.Child,
            "Senior" => showing.Prices.Senior,
            _ => 0m
        };
    }

    protected bool CanContinueToOverview => seats.All(seat => !string.IsNullOrWhiteSpace(seat.TicketType));
}
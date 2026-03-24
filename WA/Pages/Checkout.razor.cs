using MudBlazor;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Responses;

namespace WA.Pages;

using Microsoft.AspNetCore.Components;
using WA.Models;
using WA.Services.Http.Interfaces;

public partial class Checkout
{
    protected bool isLoading = true;
    protected string? errorMessage = null;

    [Inject] public IShowingApi ShowingApi { get; set; } = default!;

    protected int step = 0;

    protected List<SeatInfo> seatInfos = new List<SeatInfo>()
    {
        new SeatInfo( 2, 6, 0, 0, 0 ),
        new SeatInfo( 2, 7, 0, 0, 0 )
    };

    protected List<TicketSelection> seats = new();
    [Parameter] public int ShowingId { get; set; }

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
        try
        {
            showing = await ShowingApi.GetShowingPricesAsync(ShowingId);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
            foreach (var seatInfo in seatInfos)
            {
                seats.Add(new TicketSelection() { Row = seatInfo.Row, SeatNumber = seatInfo.Col });
            }
        }
    }

    protected bool IsTicketSelectionValid()
    {
        return seats.All(s => !string.IsNullOrEmpty(s.TicketType));
    }

    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    private void NextStep()
    {
        if (step == 1 && !IsTicketSelectionValid())
        {
            Snackbar.Add("Selecteer voor elke stoel een tickettype", Severity.Warning);
            return;
        }

        step++;
    }

    protected decimal GetSeatPrice(TicketSelection seat)
    {
        if (showing is null || string.IsNullOrWhiteSpace(seat.TicketType))
            return 0m;

        return seat.TicketType switch
        {
            "Adult" => showing.Prices.Adult.Price,
            "Student" => showing.Prices.Student.Price,
            "Child" => showing.Prices.Child.Price,
            "Senior" => showing.Prices.Senior.Price,
            _ => 0m
        };
    }

    protected bool CanContinueToOverview => seats.All(seat => !string.IsNullOrWhiteSpace(seat.TicketType));
}
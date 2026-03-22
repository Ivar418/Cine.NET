using MudBlazor;
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

    protected List<TicketSelection> seats = new()
    {
        new TicketSelection { Row = 3, SeatNumber = 6 },
        new TicketSelection { Row = 3, SeatNumber = 7 }
    };

    [Parameter] public int ShowingId { get; set; }

    protected ShowingsWithPricesResponse? showing;

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
}
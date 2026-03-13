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

    protected override async Task OnInitializedAsync()
    {
        showing = await ShowingApi.GetShowingPricesAsync(ShowingId);
    }
}
using MudBlazor;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace WA.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedLibrary.Domain.Entities;
using WA.Models;
using WA.Services.Http;
using WA.Services.Http.Interfaces;

public partial class Checkout
{
    protected bool isLoading = true;
    protected string? errorMessage = null;
    protected bool _orderBusy = false;

    [Inject] public IShowingApi ShowingApi { get; set; } = default!;
    [Inject] ISeatFinderApiClient Api { get; set; } = default!;
    [Inject] IOrderApi OrderApi { get; set; } = default!;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] IJSRuntime JS { get; set; } = default!;

    protected int step = 1;
    protected string? selectedPaymentMethod = null;
    protected CreateOrderResponse? createdOrder = null;

    protected List<SeatInfo> seatInfos = [];

    protected List<TicketSelection> seats = [];
    [Parameter] public int ShowingId { get; set; }

    protected ShowingsWithPricesResponse? showing;

    protected BookingSummary Summary => new()
    {
        MovieTitle = showing?.MovieTitle ?? string.Empty,
        StartsAt = showing?.StartsAt ?? default,
        Lines = seats,
        TotalPrice = seats.Sum(GetSeatPrice)
    };

    protected string[] paymentMethods = new[]
    {
        "Ideal",
        "Pinnen",
        "Creditcard",
        "Creditcard Online",
        "Reserveren",
        "Cadeaubon"
    };

    protected Dictionary<string, string> paymentIcons = new()
    {
        { "Ideal", Icons.Material.Filled.AccountBalance },
        { "Pinnen", Icons.Material.Filled.PointOfSale },
        { "Creditcard", Icons.Material.Filled.CreditCard },
        { "Creditcard Online", Icons.Material.Filled.Language },
        { "Reserveren", Icons.Material.Filled.Schedule },
        { "Cadeaubon", Icons.Material.Filled.CardGiftcard }
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            showing = await ShowingApi.GetShowingPricesAsync(ShowingId);
            await OnShowingChangedAsync(ShowingId);
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

    [Inject] public ISnackbar Snackbar { get; set; } = default!;

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

    private ShowingStateDto? _state;
    private int? _ShowingId;
    private bool _busy = false;
    private bool _showZones = true;
    private int _normalCount = 2;
    private int _wcCount = 0;
    private HashSet<string> _suggestedKeys = [];
    private Guid? _pendingId;
    private Reservation? _confirmedReservation = null;

    private void SetStep(int step)
    {
        this.step = step;
    }

    // ── Showing selection ────────────────────────────────────────────────
    private async Task OnShowingChangedAsync(int id)
    {
        await CancelPendingAsync();
        _ShowingId = id;
        _state = null;
        _suggestedKeys = [];
        _pendingId = null;
        seatInfos = [];
        seats = [];

        if (id != null)
        {
            _busy = true;
            _state = await Api.GetShowingStateAsync(id);
            _busy = false;
        }
    }

    // ── Suggest ───────────────────────────────────────────────────────────
    private async Task SuggestAsync()
    {
        if (!_ShowingId.HasValue || _normalCount + _wcCount == 0) return;
        await CancelPendingAsync();
        _busy = true;

        var resp = await Api.SuggestAsync(new SuggestRequest(_ShowingId.Value, _normalCount, _wcCount));
        _busy = false;

        if (resp is null || !resp.Found)
        {
            Snackbar.Add("Geen geschikte plekken beschikbaar.", Severity.Warning);
            return;
        }

        _pendingId = resp.SuggestionId;
        _suggestedKeys = resp.Seats.Select(s => $"{s.Row}-{s.Col}").ToHashSet();

        // Optimistically mark suggested seats as occupied in the local state
        if (_state is not null)
            foreach (var k in _suggestedKeys)
                _state.OccupiedKeys.Add(k);

        double avgCat = resp.Seats.Average(s => s.Category);
        Snackbar.Add($"{resp.Seats.Count} plekken gevonden (gem. cat. {avgCat:F1}). Bevestig of annuleer.",
            Severity.Info);
    }

    // ── Confirm ───────────────────────────────────────────────────────────
    private async Task ConfirmAsync()
    {
        if (!_pendingId.HasValue) return;
        var res = await Api.ConfirmAsync(_pendingId.Value);
        if (res is null)
        {
            Snackbar.Add("Bevestiging mislukt.", Severity.Error);
            return;
        }

        _suggestedKeys = [];
        _confirmedReservation = res;
        // Refresh state from server to get accurate picture
        //if (_ShowingId.HasValue)
        //    _state = await Api.GetShowingStateAsync(_ShowingId.Value);

        seatInfos = res.GetSeats();

        Snackbar.Add("Reservering bevestigd!", Severity.Success);

        foreach (var seatInfo in seatInfos)
        {
            var row = seatInfo.Row + 1;
            var col = seatInfo.Col + 1;
            seats.Add(new TicketSelection() { Row = row, SeatNumber = col });
        }
    }

    // ── Cancel ────────────────────────────────────────────────────────────
    private async Task CancelPendingAsync()
    {
        if (!_pendingId.HasValue) return;
        await Api.CancelAsync(_pendingId.Value);

        // Remove optimistic occupation
        if (_state is not null)
            foreach (var k in _suggestedKeys)
                _state.OccupiedKeys.Remove(k);

        _pendingId = null;
        _suggestedKeys = [];
        step = 1;
        await InvokeAsync(StateHasChanged);
    }

    // ── Legend ────────────────────────────────────────────────────────────
    private static readonly (string, string)[] _legend =
    [
        ("1e keuze", "#f59e0b"), ("2e keuze", "#a3e635"), ("3e keuze", "#34d399"),
        ("4e keuze", "#60a5fa"), ("5e keuze", "#c084fc"), ("6e keuze", "#f87171"),
        ("Suggestie ★", "#fbbf24"), ("Bevestigd ✓", "#22c55e"),
        ("Bezet", "#3f3f46"), ("Rolstoel ♿", "#1d4ed8"),
    ];

    // ── Step 4: pick payment method and advance ───────────────────────────
    protected void SelectPaymentMethod(string method)
    {
        selectedPaymentMethod = method;
    }

    // ── Step 5: build & submit order, then branch ─────────────────────────
    protected async Task ProcessOrderAsync()
    {
        if (string.IsNullOrWhiteSpace(selectedPaymentMethod)) return;

        _orderBusy = true;
        StateHasChanged();

        var ticketRequests = seats.Select(seat => new TicketRequest
        {
            ShowingId = ShowingId,
            ShowDateTimeUtc = showing?.StartsAt ?? DateTimeOffset.UtcNow,
            SeatNumber = $"{(char)('A' + seat.Row - 1)}{seat.SeatNumber}",
            TicketType = seat.TicketType ?? "Adult",
            Price = GetSeatPrice(seat)
        }).ToList();

        var apiPaymentMethod = selectedPaymentMethod switch
        {
            "IDEAL" => "IDEAL",
            "Creditcard" => "CREDITCARD",
            _ => "PIN"
        };

        var orderType = selectedPaymentMethod == "Reserveren" ? "Reserveren" : "Payment";

        var request = new CreateOrderRequest
        {
            OrderType = orderType,
            PaymentMethod = apiPaymentMethod,
            Tickets = ticketRequests
        };

        var order = await OrderApi.CreateOrderAsync(request);
        _orderBusy = false;

        if (order is null)
        {
            Snackbar.Add("Aanmaken van bestelling mislukt. Probeer opnieuw.", Severity.Error);
            return;
        }

        createdOrder = order;

        if (selectedPaymentMethod == "Reserveren")
        {
            await HandleReservationAsync(order);
        }
        else
        {
            GoToPaymentMock(order);
        }
    }

    // ── Reservation: download reservation PDF ────────────────────────────
    private async Task HandleReservationAsync(CreateOrderResponse order)
    {
        var pdf = await OrderApi.GetReservationPdfAsync(order.OrderId);
        if (pdf is not null)
            await DownloadPdfAsync(pdf, $"reservering-{order.OrderCode}.pdf");
        else
            Snackbar.Add("Reservering aangemaakt maar PDF kon niet worden gegenereerd.", Severity.Warning);

        Snackbar.Add($"Reservering {order.OrderCode} aangemaakt!", Severity.Success);
        var url = $"/checkout/payment-result?orderId={order.OrderId}&reservation=true";
        Nav.NavigateTo(url);
    }

    // ── Payment: redirect to iDeal mock with all required params ─────────
    private void GoToPaymentMock(CreateOrderResponse order)
    {
        var returnUrl = Uri.EscapeDataString($"/checkout/payment-result?orderId={order.OrderId}");
        // var url = $"/ideal-mock" +
        var url = $"/payment-mock" +
                  $"?reference={Uri.EscapeDataString(order.OrderCode)}" +
                  $"&amount={order.TotalAmount:F2}" +
                  $"&merchant={Uri.EscapeDataString("CineNet B.V.")}" +
                  $"&description={Uri.EscapeDataString("Bestelling " + order.OrderCode)}" +
                  $"&returnUrl={returnUrl}"+
                  $"&ChosenPaymentType={Uri.EscapeDataString(selectedPaymentMethod)}";
        Nav.NavigateTo(url);
    }

    // ── JS interop: trigger browser PDF download ──────────────────────────
    private async Task DownloadPdfAsync(byte[] bytes, string fileName)
    {
        var base64 = Convert.ToBase64String(bytes);
        await JS.InvokeVoidAsync("downloadBase64Pdf", base64, fileName);
    }
}
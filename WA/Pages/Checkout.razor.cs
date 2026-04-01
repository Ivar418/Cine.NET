using MudBlazor;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Responses;

namespace WA.Pages;

using Microsoft.AspNetCore.Components;
using SharedLibrary.Domain.Entities;
using System.ComponentModel;
using WA.Models;
using WA.Services.Http;
using WA.Services.Http.Interfaces;

public partial class Checkout
{
    protected bool isLoading = true;
    protected string? errorMessage = null;

    [Inject] public IShowingApi ShowingApi { get; set; } = default!;
    [Inject] ISeatFinderApiClient Api { get; set; } = default!;

    protected int step = 1;

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
        Snackbar.Add($"{resp.Seats.Count} plekken gevonden (gem. cat. {avgCat:F1}). Bevestig of annuleer.", Severity.Info);
    }

    // ── Confirm ───────────────────────────────────────────────────────────
    private async Task ConfirmAsync()
    {
        if (!_pendingId.HasValue) return;
        var res = await Api.ConfirmAsync(_pendingId.Value);
        if (res is null) { Snackbar.Add("Bevestiging mislukt.", Severity.Error); return; }

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

        step++;

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

    // Toggle a single seat as suggested or not. This updates local counts (_normalCount/_wcCount)
    // and then requests an updated suggestion from the API so the reservation state stays in sync.
    private async Task HandleSeatClickAsync(SeatInfo seat)
    {
        if (_state is null || !_ShowingId.HasValue) return;

        var key = $"{seat.Row}-{seat.Col}";

        // If the seat is occupied (and not currently part of our suggested set), ignore clicks
        var occupied = _state.OccupiedKeys.Contains(key);
        if (occupied && !_suggestedKeys.Contains(key))
            return;

        if (_suggestedKeys.Count() == 0)
        {
            _normalCount = 0;
            _wcCount = 0;
        }

        // Toggle selection
        if (_suggestedKeys.Contains(key))
        {
            _suggestedKeys.Remove(key);
            if (seat.Type == SeatType.Wheelchair)
                _wcCount = Math.Max(0, _wcCount - 1);
            else
                _normalCount = Math.Max(0, _normalCount - 1);
        }
        else
        {
            _suggestedKeys.Add(key);
            if (seat.Type == SeatType.Wheelchair)
                _wcCount++;
            else
                _normalCount++;
        }

        // If we have an active pending suggestion, update that reservation's seats on the server
        // to add/remove the clicked seat while preserving the other suggested seats.
        if (_pendingId.HasValue)
        {
            // Build the list of SeatInfo objects from the current _suggestedKeys
            var seatsToSend = _suggestedKeys
                .Select(k => {
                    var parts = k.Split('-');
                    if (parts.Length != 2) return null;
                    if (!int.TryParse(parts[0], out var r)) return null;
                    if (!int.TryParse(parts[1], out var c)) return null;
                    return _state.AllSeats.FirstOrDefault(s => s.Row == r && s.Col == c);
                })
                .Where(s => s is not null)
                .Select(s => s!)
                .ToList();

            var updated = await Api.UpdateReservationSeatsAsync(_pendingId.Value, seatsToSend);
            if (updated is null)
            {
                Snackbar.Add("Bijwerken van reservering mislukt.", Severity.Error);
                // As a fallback, cancel the pending suggestion locally
                await CancelPendingAsync();
            }
            else
            {
                // Refresh the showing state so occupied keys are accurate
                if (_ShowingId.HasValue)
                    _state = await Api.GetShowingStateAsync(_ShowingId.Value);

                Snackbar.Add("Reservering bijgewerkt.", Severity.Info);
            }
        }
        else
        {
            // No pending suggestion yet -> ask server to suggest based on counts
            var id = await Api.SuggestAsync(new SuggestRequest(_ShowingId.Value, _normalCount, _wcCount));
            _pendingId = id.SuggestionId;
        }

        await InvokeAsync(StateHasChanged);
    }

    // ── Legend ────────────────────────────────────────────────────────────
    private static readonly (string, string)[] _legend =
    [
        ("1e keuze", "#f59e0b"), ("2e keuze", "#a3e635"), ("3e keuze", "#34d399"),
        ("4e keuze", "#60a5fa"), ("5e keuze", "#c084fc"), ("6e keuze", "#f87171"),
        ("Suggestie ★", "#fbbf24"), ("Bevestigd ✓", "#22c55e"),
        ("Bezet", "#3f3f46"),    ("Rolstoel ♿", "#1d4ed8"),
    ];
}
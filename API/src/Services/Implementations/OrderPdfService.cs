using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace API.Services.Implementations;

public class OrderPdfService : IOrderPdfService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShowingRepository _showingRepository;

    public OrderPdfService(IOrderRepository orderRepository, IShowingRepository showingRepository)
    {
        _orderRepository = orderRepository;
        _showingRepository = showingRepository;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<ResultOf<byte[]>> GenerateReservationPdfAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<byte[]>.Failure($"Order with id {orderId} was not found.");

        // Load showing info for the first ticket (reservation is per order, not per ticket)
        var firstTicket = order.OrderTickets.FirstOrDefault()?.Ticket;
        ShowingInfo? showingInfo = null;

        if (firstTicket is not null)
        {
            var showingResult = await _showingRepository.GetShowingDisplayByIdAsync(firstTicket.ShowingId);
            if (showingResult.IsSuccess && showingResult.Value is not null)
            {
                showingInfo = new ShowingInfo(
                    showingResult.Value.MovieTitle,
                    showingResult.Value.AuditoriumName,
                    showingResult.Value.StartsAt
                );
            }
        }

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(header =>
                {
                    header.Text("Cine.NET — Reservering")
                        .SemiBold()
                        .FontSize(22)
                        .FontColor(Colors.Blue.Darken2);
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Item().Text("Bedankt voor uw reservering!").SemiBold().FontSize(16);
                    col.Item().PaddingTop(10).Text($"Reserveringscode: {order.OrderCode}").SemiBold();
                    col.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    if (showingInfo is not null)
                    {
                        col.Item().PaddingTop(15).Text($"Film: {showingInfo.MovieTitle}").FontSize(14);
                        col.Item().PaddingTop(5).Text($"Zaal: {showingInfo.AuditoriumName}");
                        col.Item().PaddingTop(5).Text($"Datum/Tijd: {showingInfo.StartsAt:dddd d MMMM yyyy HH:mm}");
                    }

                    col.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingTop(15).Text($"Totaalbedrag: €{order.TotalAmount:F2}");
                    col.Item().PaddingTop(5).Text($"Betaalmethode: {order.PaymentMethod}");
                    col.Item().PaddingTop(5).Text($"Aangemaakt op: {order.CreatedAtUtc:dd-MM-yyyy HH:mm}");

                    col.Item().PaddingTop(30).Text(
                        "Laat deze reserveringscode zien aan de kassa om uw tickets op te halen.")
                        .Italic()
                        .FontColor(Colors.Grey.Darken1);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Cine.NET — ").FontColor(Colors.Grey.Medium);
                    text.Span($"Reservering #{order.Id}").FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();

        return ResultOf<byte[]>.Success(pdfBytes);
    }

    public async Task<ResultOf<byte[]>> GeneratePaidTicketsPdfAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<byte[]>.Failure($"Order with id {orderId} was not found.");

        if (!order.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            return ResultOf<byte[]>.Failure("Tickets PDF can only be generated for paid orders.");

        var tickets = order.OrderTickets
            .Where(ot => ot.Ticket is not null)
            .Select(ot => ot.Ticket!)
            .ToList();

        if (tickets.Count == 0)
            return ResultOf<byte[]>.Failure("No tickets found for this order.");

        // Load showing info per unique showing
        var showingCache = new Dictionary<int, ShowingInfo>();
        foreach (var ticket in tickets)
        {
            if (showingCache.ContainsKey(ticket.ShowingId)) continue;
            var showingResult = await _showingRepository.GetShowingDisplayByIdAsync(ticket.ShowingId);
            if (showingResult.IsSuccess && showingResult.Value is not null)
            {
                showingCache[ticket.ShowingId] = new ShowingInfo(
                    showingResult.Value.MovieTitle,
                    showingResult.Value.AuditoriumName,
                    showingResult.Value.StartsAt
                );
            }
        }

        var pdfBytes = Document.Create(container =>
        {
            foreach (var ticket in tickets)
            {
                showingCache.TryGetValue(ticket.ShowingId, out var showing);

                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Element(header =>
                    {
                        header.Text("Cine.NET — Ticket")
                            .SemiBold()
                            .FontSize(22)
                            .FontColor(Colors.Blue.Darken2);
                    });

                    page.Content().PaddingTop(20).Column(col =>
                    {
                        col.Item().Text("Uw ticket").SemiBold().FontSize(16);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        if (showing is not null)
                        {
                            col.Item().PaddingTop(15).Text($"Film: {showing.MovieTitle}").FontSize(14).SemiBold();
                            col.Item().PaddingTop(5).Text($"Zaal: {showing.AuditoriumName}");
                            col.Item().PaddingTop(5).Text($"Datum/Tijd: {showing.StartsAt:dddd d MMMM yyyy HH:mm}");
                        }

                        col.Item().PaddingTop(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(15).Text($"Stoel: {ticket.SeatNumber}");
                        col.Item().PaddingTop(5).Text($"Tickettype: {ticket.TicketType}");
                        col.Item().PaddingTop(5).Text($"Prijs: €{ticket.Price:F2}");
                        col.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(15).Text("Ticketcode (QR):").SemiBold();
                        col.Item().PaddingTop(5).Text(ticket.QrCodeGuid ?? "N/A")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken2);
                        col.Item().PaddingTop(5).Text($"Ordernummer: {order.OrderCode}").FontSize(10);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Cine.NET — ").FontColor(Colors.Grey.Medium);
                        text.Span($"Ticket #{ticket.Id}").FontColor(Colors.Grey.Medium);
                    });
                });
            }
        }).GeneratePdf();

        return ResultOf<byte[]>.Success(pdfBytes);
    }

    private record ShowingInfo(string MovieTitle, string AuditoriumName, DateTimeOffset StartsAt);
}


using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IShowingRepository _showingRepository;

    public OrderService(
        ITicketRepository ticketRepository,
        IOrderRepository orderRepository,
        IShowingRepository showingRepository)
    {
        _ticketRepository = ticketRepository;
        _orderRepository = orderRepository;
        _showingRepository = showingRepository;
    }

    public async Task<ResultOf<CreateOrderResponse>> CreateAsync(CreateOrderRequest request)
    {
        if (request.Tickets is null || request.Tickets.Count == 0)
            return ResultOf<CreateOrderResponse>.Failure("At least one ticket is required.");

        if (string.IsNullOrWhiteSpace(request.OrderType))
            return ResultOf<CreateOrderResponse>.Failure("OrderType is required.");

        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            return ResultOf<CreateOrderResponse>.Failure("PaymentMethod is required.");

        foreach (var reqTicket in request.Tickets)
        {
            if (reqTicket.ShowingId <= 0)
                return ResultOf<CreateOrderResponse>.Failure("Each ticket must have a valid ShowingId.");
            if (string.IsNullOrWhiteSpace(reqTicket.SeatNumber))
                return ResultOf<CreateOrderResponse>.Failure("Each ticket must include a SeatNumber.");
            if (string.IsNullOrWhiteSpace(reqTicket.TicketType))
                return ResultOf<CreateOrderResponse>.Failure("Each ticket must include a TicketType.");
            if (reqTicket.Price < 0)
                return ResultOf<CreateOrderResponse>.Failure("Ticket price cannot be negative.");

            var showingResult = await _showingRepository.GetShowingAsync(reqTicket.ShowingId);
            if (!showingResult.IsSuccess || showingResult.Value is null)
                return ResultOf<CreateOrderResponse>.Failure($"Showing with id {reqTicket.ShowingId} does not exist.");
        }

        var paymentStatus = "Pending";
        var totalAmount = request.Tickets.Sum(t => t.Price);

        var persistedTickets = new List<Ticket>();
        foreach (var reqTicket in request.Tickets)
        {
            var ticket = new Ticket
            {
                ShowingId = reqTicket.ShowingId,
                ShowDateTimeUtc = reqTicket.ShowDateTimeUtc.ToString("O"),
                SeatNumber = reqTicket.SeatNumber,
                Price = reqTicket.Price,
                TicketType = reqTicket.TicketType,
                PaymentStatus = paymentStatus
            };

            await _ticketRepository.AddAsync(ticket);
            persistedTickets.Add(ticket);
        }

        var order = new Order
        {
            OrderCode = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant(),
            CreatedAtUtc = DateTime.UtcNow,
            TotalAmount = totalAmount,
            OrderType = request.OrderType,
            PaymentStatus = paymentStatus,
            PaymentMethod = request.PaymentMethod,
            IsPrinted = false
        };

        order.OrderTickets = persistedTickets
            .Select(t => new OrderTicket { TicketId = t.Id, Ticket = t })
            .ToList();

        await _orderRepository.AddAsync(order);

        var response = new CreateOrderResponse
        {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatus,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = persistedTickets.Select(t => new CreatedOrderTicketResponse
            {
                TicketId = t.Id,
                ShowingId = t.ShowingId,
                SeatNumber = t.SeatNumber,
                TicketType = t.TicketType,
                Price = t.Price,
                PaymentStatus = t.PaymentStatus,
                TicketCode = t.QrCodeGuid
            }).ToList()
        };

        return ResultOf<CreateOrderResponse>.Success(response);
    }

    public async Task<ResultOf<CreateOrderResponse>> ConfirmPaymentAsync(int orderId)
    {
        if (orderId <= 0)
            return ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0.");

        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<CreateOrderResponse>.Failure($"Order with id {orderId} was not found.");

        if (!string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
        {
            order.PaymentStatus = "Paid";

            foreach (var orderTicket in order.OrderTickets)
            {
                if (orderTicket.Ticket is null) continue;
                orderTicket.Ticket.PaymentStatus = "Paid";
                orderTicket.Ticket.QrIsActive = true;
            }

            await _orderRepository.SaveChangesAsync();
        }

        var response = new CreateOrderResponse
        {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatus,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = order.OrderTickets
                .Where(ot => ot.Ticket is not null)
                .Select(ot => new CreatedOrderTicketResponse
                {
                    TicketId = ot.TicketId,
                    ShowingId = ot.Ticket!.ShowingId,
                    SeatNumber = ot.Ticket.SeatNumber,
                    TicketType = ot.Ticket.TicketType,
                    Price = ot.Ticket.Price,
                    PaymentStatus = ot.Ticket.PaymentStatus,
                    TicketCode = ot.Ticket.QrCodeGuid
                })
                .ToList()
        };

        return ResultOf<CreateOrderResponse>.Success(response);
    }

    public async Task<ResultOf<CreateOrderResponse>> GetByIdAsync(int orderId)
    {
        if (orderId <= 0)
            return ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0.");

        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<CreateOrderResponse>.Failure($"Order with id {orderId} was not found.");

        var response = new CreateOrderResponse
        {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatus,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = order.OrderTickets
                .Where(ot => ot.Ticket is not null)
                .Select(ot => new CreatedOrderTicketResponse
                {
                    TicketId = ot.TicketId,
                    ShowingId = ot.Ticket!.ShowingId,
                    SeatNumber = ot.Ticket.SeatNumber,
                    TicketType = ot.Ticket.TicketType,
                    Price = ot.Ticket.Price,
                    PaymentStatus = ot.Ticket.PaymentStatus,
                    TicketCode = ot.Ticket.QrCodeGuid
                })
                .ToList()
        };

        return ResultOf<CreateOrderResponse>.Success(response);
    }

    public async Task<ResultOf<CreateOrderResponse>> ResetToPendingAsync(int orderId)
    {
        if (orderId <= 0)
            return ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0.");

        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<CreateOrderResponse>.Failure($"Order with id {orderId} was not found.");

        order.PaymentStatus = "Pending";

        foreach (var orderTicket in order.OrderTickets)
        {
            if (orderTicket.Ticket is null) continue;
            orderTicket.Ticket.PaymentStatus = "Pending";
            orderTicket.Ticket.QrIsActive = false;
        }

        await _orderRepository.SaveChangesAsync();

        var response = new CreateOrderResponse
        {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatus,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = order.OrderTickets
                .Where(ot => ot.Ticket is not null)
                .Select(ot => new CreatedOrderTicketResponse
                {
                    TicketId = ot.TicketId,
                    ShowingId = ot.Ticket!.ShowingId,
                    SeatNumber = ot.Ticket.SeatNumber,
                    TicketType = ot.Ticket.TicketType,
                    Price = ot.Ticket.Price,
                    PaymentStatus = ot.Ticket.PaymentStatus,
                    TicketCode = ot.Ticket.QrCodeGuid
                })
                .ToList()
        };

        return ResultOf<CreateOrderResponse>.Success(response);
    }
}
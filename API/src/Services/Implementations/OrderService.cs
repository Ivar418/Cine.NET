using API.Domain.Common;
using API.Mappers;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.Domain.Entities.Enums;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;

namespace API.Services.Implementations;

public class OrderService : IOrderService {
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IShowingRepository _showingRepository;

    public OrderService(
        ITicketRepository ticketRepository,
        IOrderRepository orderRepository,
        IShowingRepository showingRepository) {
        _ticketRepository = ticketRepository;
        _orderRepository = orderRepository;
        _showingRepository = showingRepository;
    }

    /// <summary>
    /// Validates ticket and payment input, persists tickets and order data, and returns a composed order response.
    /// </summary>
    /// <param name="request">The order creation payload.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the created order response,
    /// or a failure when validation or persistence cannot be completed.
    /// </returns>
    public async Task<ResultOf<CreateOrderResponse>> CreateAsync(CreateOrderRequest request) {
        foreach (var reqTicket in request.Tickets) {
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

        var paymentStatus = PaymentStatuses.Pending;
        var totalAmount = request.Tickets.Sum(t => t.Price);

        var persistedTickets = new List<Ticket>();
        foreach (var reqTicket in request.Tickets) {
            var ticket = new Ticket {
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

        var order = new Order {
            OrderCode = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant(),
            CreatedAtUtc = DateTime.UtcNow,
            TotalAmount = totalAmount,
            OrderType = request.OrderType,
            PaymentStatuses = paymentStatus,
            PaymentMethod = request.PaymentMethod,
            IsPrinted = false,
            UserId = request.UserId ?? null
        };

        order.OrderTickets = persistedTickets
            .Select(t => new OrderTicket { TicketId = t.Id, Ticket = t })
            .ToList();

        await _orderRepository.AddAsync(order);

        var response = new CreateOrderResponse {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatuses,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = persistedTickets.Select(t => new CreatedOrderTicketResponse {
                TicketId = t.Id,
                ShowingId = t.ShowingId,
                SeatNumber = t.SeatNumber,
                TicketType = t.TicketType,
                Price = t.Price,
                PaymentStatus = t.PaymentStatus,
                TicketCode = t.QrCodeGuid
            }).ToList(),
            UserId = order.UserId
        };

        return ResultOf<CreateOrderResponse>.Success(response);
    }

    /// <summary>
    /// Marks an order and its tickets as paid and activates QR usage for associated tickets.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the updated order response,
    /// or a failure when the order is invalid or not found.
    /// </returns>
    public async Task<ResultOf<CreateOrderResponse>> ConfirmPaymentAsync(int orderId) {
        if (orderId <= 0)
            return ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0.");

        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<CreateOrderResponse>.Failure($"Order with id {orderId} was not found.");

        if (order.PaymentStatuses != PaymentStatuses.Paid) {
            order.PaymentStatuses = PaymentStatuses.Paid;

            foreach (var orderTicket in order.OrderTickets) {
                if (orderTicket.Ticket is null) continue;
                orderTicket.Ticket.PaymentStatus = PaymentStatuses.Paid;
                orderTicket.Ticket.QrIsActive = true;
            }

            await _orderRepository.SaveChangesAsync();
        }

        var response = new CreateOrderResponse {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatuses,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = order.OrderTickets
                .Where(ot => ot.Ticket is not null)
                .Select(ot => new CreatedOrderTicketResponse {
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

    /// <summary>
    /// Retrieves a single order with tickets and maps it to the public order response shape.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the order response,
    /// or a failure when the identifier is invalid or the order does not exist.
    /// </returns>
    public async Task<ResultOf<CreateOrderResponse>> GetByIdAsync(int orderId) {
        if (orderId <= 0)
            return ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0.");

        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<CreateOrderResponse>.Failure($"Order with id {orderId} was not found.");

        var response = new CreateOrderResponse {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatuses,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = order.OrderTickets
                .Where(ot => ot.Ticket is not null)
                .Select(ot => new CreatedOrderTicketResponse {
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

    /// <summary>
    /// Retrieves all orders with tickets and maps each record to the public order response shape.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing all mapped order responses.
    /// </returns>
    public async Task<ResultOf<List<CreateOrderResponse>>> GetAllAsync() {
        var orders = await _orderRepository.GetAllWithTicketsAsync();

        var responses = orders.Select(order => new CreateOrderResponse {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatuses,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = order.OrderTickets
                .Where(ot => ot.Ticket is not null)
                .Select(ot => new CreatedOrderTicketResponse {
                    TicketId = ot.TicketId,
                    ShowingId = ot.Ticket!.ShowingId,
                    SeatNumber = ot.Ticket.SeatNumber,
                    TicketType = ot.Ticket.TicketType,
                    Price = ot.Ticket.Price,
                    PaymentStatus = ot.Ticket.PaymentStatus,
                    TicketCode = ot.Ticket.QrCodeGuid
                }).ToList()
        }).ToList();

        return ResultOf<List<CreateOrderResponse>>.Success(responses);
    }

    public async Task<ResultOf<List<CreateOrderResponse>>> GetOrdesByUserId(int userId) {
        var result = await _orderRepository.GetAllOrdersByUserId(userId: userId);
        if (result.IsSuccess) {
            return ResultOf<List<CreateOrderResponse>>.Success(result.Value?.Select(o => OrderMapper.ToResponse(o))
                .ToList() ?? new List<CreateOrderResponse>());
        }

        return ResultOf<List<CreateOrderResponse>>.Failure(result.Error ?? "Something wen wrong fetching orders");
    }

    /// <summary>
    /// Resets an order and all related tickets back to pending payment state and deactivates ticket QR usage.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the updated order response,
    /// or a failure when the identifier is invalid or the order does not exist.
    /// </returns>
    public async Task<ResultOf<CreateOrderResponse>> ResetToPendingAsync(int orderId) {
        if (orderId <= 0)
            return ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0.");

        var order = await _orderRepository.GetByIdWithTicketsAsync(orderId);
        if (order is null)
            return ResultOf<CreateOrderResponse>.Failure($"Order with id {orderId} was not found.");

        order.PaymentStatuses = PaymentStatuses.Pending;

        foreach (var orderTicket in order.OrderTickets) {
            if (orderTicket.Ticket is null) continue;
            orderTicket.Ticket.PaymentStatus = PaymentStatuses.Pending;
            orderTicket.Ticket.QrIsActive = false;
        }

        await _orderRepository.SaveChangesAsync();

        var response = new CreateOrderResponse {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrderType = order.OrderType,
            PaymentStatus = order.PaymentStatuses,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            Tickets = order.OrderTickets
                .Where(ot => ot.Ticket is not null)
                .Select(ot => new CreatedOrderTicketResponse {
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
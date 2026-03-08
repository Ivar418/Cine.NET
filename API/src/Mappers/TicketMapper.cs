using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Requests;

namespace API.Mappers;

public class TicketMapper
{
    public static TicketResponse ToResponse(Ticket ticket)
    {
        return new TicketResponse
        {
            Id = ticket.Id,
            MovieId = ticket.MovieId,
            MovieTitle = ticket.Movie.Title,
            ShowDateTime = ticket.ShowDateTime,
            SeatNumber = ticket.SeatNumber,
            Status = ticket.Status,
            Price = ticket.Price,
            PurchaseDate = ticket.PurchaseDate
        };
    }

    public static IEnumerable<TicketResponse> ToResponse(IEnumerable<Ticket> tickets)
    {
        return tickets.Select(ToResponse);
    }

    public static Ticket ToEntity(TicketRequest request)
    {
        return new Ticket(request.MovieId, request.ShowDateTime, request.SeatNumber, request.Price);
    }
}
using API.Domain.Common;

namespace API.Services.Interfaces;

public interface IOrderPdfService
{
    Task<ResultOf<byte[]>> GenerateReservationPdfAsync(int orderId);
    Task<ResultOf<byte[]>> GeneratePaidTicketsPdfAsync(int orderId);
}


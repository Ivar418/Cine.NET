using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    public enum ReservationStatus { Pending, Confirmed, Cancelled }

    public record ReservationDto(
        Guid Id,
        int ShowtimeId,
        List<SeatInfo> Seats,
        ReservationStatus Status,
        DateTimeOffset CreatedAt
    );

    public record SuggestRequest(int ShowtimeId, int NormalCount, int WheelchairCount);

    public record SuggestResponse(Guid SuggestionId, List<SeatInfo> Seats, bool Found);

    public record ConfirmRequest(Guid SuggestionId);
    public record CancelRequest(Guid ReservationId);
}

using System;
using System.Collections.Generic;

namespace SharedLibrary.DTOs.Models
{
    public record UpdateReservationSeatsRequest(Guid SuggestionId, List<SeatInfo> Seats);
}

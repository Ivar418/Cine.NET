using SharedLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    public record ShowingStateDto(
    Showing Showing,
    List<SeatInfo> AllSeats,       // derived from the stored Auditorium-layout snapshot
    HashSet<string> OccupiedKeys    // "row-col" of confirmed seats
    );

    public record CreateShowingRequest(int MovieId, int AuditoriumId, DateTimeOffset StartsAt, bool Is3D = false);
}

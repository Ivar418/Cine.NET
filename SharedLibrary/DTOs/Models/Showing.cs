using SharedLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    public record ShowingStateDto(
    ShowingDto Showing,
    List<SeatInfo> AllSeats,       // derived from the stored Auditorium-layout snapshot
    HashSet<string> OccupiedKeys    // "row-col" of confirmed seats
    );

    public record ShowingDto(
    int Id,
    Movie Movie,
    AuditoriumDto Auditorium,
    DateTimeOffset StartsAt
    );

    public record CreateShowingRequest(int MovieId, int AuditoriumId, DateTimeOffset StartsAt, bool Is3D = false);
}

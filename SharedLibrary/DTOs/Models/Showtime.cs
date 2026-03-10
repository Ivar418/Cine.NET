using SharedLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    public record ShowtimeStateDto(
    ShowtimeDto Showtime,
    List<SeatInfo> AllSeats,       // derived from the stored hall-layout snapshot
    HashSet<string> OccupiedKeys    // "row-col" of confirmed seats
    );

    public record ShowtimeDto(
    int Id,
    Movie Movie,
    HallDto Hall,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt        // = StartsAt + Film.DurationMinutes
    );

    public record CreateShowtimeRequest(int MovieId, int HallId, DateTimeOffset StartsAt);
}

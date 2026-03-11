using SharedLibrary.DTOs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SharedLibrary.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ShowingId { get; set; }
        public string SeatsJson { get; set; } = "[]";   // List<SeatInfo> as JSON
        public string Status { get; set; } = "Pending"; // Pending | Confirmed | Cancelled
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public List<SeatInfo> GetSeats() =>
            JsonSerializer.Deserialize<List<SeatInfo>>(SeatsJson) ?? [];

        public void SetSeats(IEnumerable<SeatInfo> seats) =>
            SeatsJson = JsonSerializer.Serialize(seats.ToList());

        // Navigation
        public Showing Showing { get; set; } = default!;
    }
}

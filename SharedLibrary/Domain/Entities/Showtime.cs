using SharedLibrary.DTOs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SharedLibrary.Domain.Entities
{
    public class Showtime
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public int HallId { get; set; }
        public DateTimeOffset StartsAt { get; set; }

        /// <summary>
        /// Snapshot of the hall's row configuration at the moment the showtime was created.
        /// This protects against later edits to the hall affecting existing showtimes.
        /// (Optie C: JSON-snapshot + aparte reserveringen)
        /// </summary>
        public string HallLayoutSnapshot { get; set; } = "[]";

        public List<RowConfig> GetLayoutSnapshot() =>
            JsonSerializer.Deserialize<List<RowConfig>>(HallLayoutSnapshot) ?? [];

        public void SetLayoutSnapshot(IEnumerable<RowConfig> rows) =>
            HallLayoutSnapshot = JsonSerializer.Serialize(rows.ToList());

        // Navigation
        public Movie Movie { get; set; } = default!;
        public Hall Hall { get; set; } = default!;
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}

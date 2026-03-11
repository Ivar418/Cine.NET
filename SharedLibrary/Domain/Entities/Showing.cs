using SharedLibrary.DTOs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SharedLibrary.Domain.Entities
{
    public class Showing
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int AuditoriumId { get; set; }
        public DateTimeOffset StartsAt { get; set; }

        /// <summary>
        /// Snapshot of the Auditorium's row configuration at the moment the Showing was created.
        /// This protects against later edits to the Auditorium affecting existing Showings.
        /// (Optie C: JSON-snapshot + aparte reserveringen)
        /// </summary>
        public string AuditoriumLayoutSnapshot { get; set; } = "[]";

        public List<RowConfig> GetLayoutSnapshot() =>
            JsonSerializer.Deserialize<List<RowConfig>>(AuditoriumLayoutSnapshot) ?? [];

        public void SetLayoutSnapshot(IEnumerable<RowConfig> rows) =>
            AuditoriumLayoutSnapshot = JsonSerializer.Serialize(rows.ToList());

        // Navigation
        public Movie Movie { get; set; } = default!;
        public Auditorium Auditorium { get; set; } = default!;
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}

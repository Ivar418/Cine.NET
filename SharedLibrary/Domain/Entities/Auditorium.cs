using SharedLibrary.DTOs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SharedLibrary.Domain.Entities
{
    public class Auditorium
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        /// <summary>
        /// Row configurations stored as JSON.
        /// e.g. [{"Seats":12,"Wheelchair":2}, ...]
        /// </summary>
        public string RowConfigJson { get; set; } = "[]";

        public List<RowConfig> GetRows() =>
            JsonSerializer.Deserialize<List<RowConfig>>(RowConfigJson) ?? [];

        public void SetRows(IEnumerable<RowConfig> rows) =>
            RowConfigJson = JsonSerializer.Serialize(rows.ToList());

        // Navigation
        //public ICollection<Showing> Showings { get; set; } = [];
    }
}

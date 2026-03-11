using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    public record AuditoriumDto(int Id, string Name, List<RowConfig> Rows);
    public record CreateAuditoriumRequest(string Name, List<RowConfig> Rows);
    public record UpdateAuditoriumRequest(string Name, List<RowConfig> Rows);
}

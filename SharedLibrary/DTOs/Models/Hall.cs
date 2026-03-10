using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    public record HallDto(int Id, string Name, List<RowConfig> Rows);
    public record CreateHallRequest(string Name, List<RowConfig> Rows);
    public record UpdateHallRequest(string Name, List<RowConfig> Rows);
}

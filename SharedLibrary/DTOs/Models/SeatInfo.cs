using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    /// <summary>
    /// Represents a seat with detailed information including its physical position,
    /// virtual column for zone calculations, type, and category indicating preference.
    /// </summary>
    public record SeatInfo(int Row, int Col, int VirtualCol, SeatType Type, int Category);
}

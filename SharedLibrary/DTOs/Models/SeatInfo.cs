using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    /// <summary>
    /// A fully resolved seat: physical position, virtual column (for zone calc),
    /// type and preference category 1–6.
    /// </summary>
    public record SeatInfo(int Row, int Col, int VirtualCol, SeatType Type, int Category);
}

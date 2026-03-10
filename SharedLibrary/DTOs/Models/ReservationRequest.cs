using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    /// <summary>
    /// A reservation request for a (possibly mixed) group.
    /// If NormalCount > 1, at least 2 normal seats must be side-by-side.
    /// If mixed, normal seats must be directly beside the wheelchair seat(s).
    /// </summary>
    public record ReservationRequest(int NormalCount, int WheelchairCount)
    {
        public int Total => NormalCount + WheelchairCount;
        public bool IsMixed => NormalCount > 0 && WheelchairCount > 0;
        public bool NormalOnly => WheelchairCount == 0;
        public bool WheelchairOnly => NormalCount == 0;
    }
}

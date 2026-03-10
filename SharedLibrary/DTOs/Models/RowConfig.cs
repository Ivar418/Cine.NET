using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DTOs.Models
{
    /// <summary>
    /// One row in a hall: total number of seats and how many are wheelchair places.
    /// Wheelchair places always occupy the ends of the row
    /// (ceil(wc/2) on the left, floor(wc/2) on the right).
    /// </summary>
    public record RowConfig(int Seats, int Wheelchair)
    {
        public int LeftWheelchair => (Wheelchair + 1) / 2;
        public int RightWheelchair => Wheelchair / 2;
    }
}

using System;

namespace GasMonPersonal.Models
{
    public class AverageReading
    {
        public string LocationId { get; set; }
        
        public double Value { get; set; }
        
        public DateTime IntervalStartTime { get; set; }
        
        public DateTime IntervalEndTime { get; set; }
    }
}
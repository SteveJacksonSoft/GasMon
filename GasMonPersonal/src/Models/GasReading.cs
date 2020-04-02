using System;
using GasMonPersonal.DateTimeConversion;

namespace GasMonPersonal.Models
{
    public class GasReading
    {
        public string LocationId { get; set; }

        public string EventId { get; set; }

        public double Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public GasReading(GasReadingJson gasReadingJson)
        {
            LocationId = gasReadingJson.LocationId;
            EventId = gasReadingJson.EventId;
            Value = gasReadingJson.Value;
            TimeStamp = gasReadingJson.TimeStamp.ToDateTime();
        }
    }
}
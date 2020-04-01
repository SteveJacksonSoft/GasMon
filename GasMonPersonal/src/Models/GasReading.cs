namespace GasMonPersonal.Models
{
    public class GasReading
    {
        public string LocationId { get; set; }

        public string EventId { get; set; }

        public double Value { get; set; }

        public long TimeStamp { get; set; } 
    }
}
namespace GasMonPersonal.Models
{
    public class GasMessage
    {
        public string LocationId { get; set; }

        public string EventId { get; set; }

        public double Value { get; set; }

        public long Timestamp { get; set; } 
    }
}
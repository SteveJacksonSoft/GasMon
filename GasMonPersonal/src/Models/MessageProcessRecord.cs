using System;

namespace GasMonPersonal.Models
{
    public class MessageProcessRecord
    {
        public string EventId { get; set; } 
        public DateTime ProcessTime { get; set; }

        public MessageProcessRecord(GasReading reading)
        {
            this.EventId = reading.EventId;
            this.ProcessTime = DateTime.Now;
        }
    }
}
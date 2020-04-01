using System;

namespace GasMonPersonal.Models
{
    public class MessageProcessRecord
    {
        public string EventId { get; set; } 
        public DateTime ProcessTime { get; set; }

        public MessageProcessRecord(GasMessage message)
        {
            this.EventId = message.EventId;
            this.ProcessTime = DateTime.Now;
        }
    }
}
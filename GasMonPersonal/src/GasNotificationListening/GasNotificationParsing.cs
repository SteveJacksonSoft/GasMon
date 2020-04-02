using GasMonPersonal.Models;
using Newtonsoft.Json;

namespace GasMonPersonal.GasNotificationListening
{
    public static class GasNotificationParsing
    {
        public static GasReading ExtractReading(string notification)
        {
            var message = JsonConvert.DeserializeAnonymousType(notification, new {Message = ""}).Message;
            return new GasReading(
                JsonConvert.DeserializeObject<GasReadingJson>(message)
            );
        }
    }
}
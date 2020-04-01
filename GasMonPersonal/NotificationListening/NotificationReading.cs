using GasMonPersonal.Models;
using Newtonsoft.Json;

namespace GasMonPersonal.NotificationListening
{
    public static class NotificationReading
    {
        public static GasMessage ExtractMessage(string notification)
        {
            var message = JsonConvert.DeserializeAnonymousType(notification, new {Message = ""}).Message;
            return JsonConvert.DeserializeObject<GasMessage>(
                message
            );
        }
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GasMonPersonal.AWS;
using GasMonPersonal.Locations;
using GasMonPersonal.MessageProcessing;
using GasMonPersonal.NotificationListening;

namespace GasMonPersonal
{
    public static class GasMonApp
    {
        public static async Task Run()
        {
            var apiClient = new AwsApiClient();

            var locations = await LocationFetching.FetchLocations(apiClient);
            
            using var messageProcessor = new MessageProcessor(locations.ToList());

            await using var notificationManager = new NotificationManager(apiClient, messageProcessor.ProcessMessage);
            
            await notificationManager.StartProcessingGasNotifications();
            
            Thread.Sleep(20_000);
        }
    }
}
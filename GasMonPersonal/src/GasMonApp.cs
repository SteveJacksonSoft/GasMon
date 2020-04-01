using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GasMonPersonal.Locations;
using GasMonPersonal.MessageProcessing;
using GasMonPersonal.NotificationListening;

namespace GasMonPersonal
{
    public static class GasMonApp
    {
        public static async Task Run()
        {
            var locationIds = (await LocationFetching.FetchLocations()).Select(location => location.Id);

            using var duplicateChecker = new DuplicateChecker();

            var messageProcessor = new MessageProcessor(
                duplicateChecker,
                message => locationIds.Contains(message.LocationId)
            );

            await using var notificationManager = new NotificationManager(messageProcessor.ProcessMessage);

            await notificationManager.StartProcessingGasNotifications();

            Thread.Sleep(20_000);
        }
    }
}
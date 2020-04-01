using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GasMonPersonal.Locations;
using GasMonPersonal.ReadingCollecting;
using GasMonPersonal.GasNotificationListening;

namespace GasMonPersonal
{
    public static class GasMonApp
    {
        public static async Task Run()
        {
            var locationIds = (await LocationFetching.FetchLocations()).Select(location => location.Id);

            using var duplicateChecker = new DuplicateChecker();

            var readingCollector = new ReadingCollector(
                duplicateChecker,
                message => locationIds.Contains(message.LocationId)
            );

            await using var gasReadingListener = new GasReadingListener(readingCollector.SaveMessageIfValid);

            await gasReadingListener.StartListeningForGasReadings();

            Thread.Sleep(20_000);
        }
    }
}
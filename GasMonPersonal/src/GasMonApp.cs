using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GasMonPersonal.GasNotificationListening;
using GasMonPersonal.Locations;
using GasMonPersonal.ReadingCollecting;
using Timer = System.Timers.Timer;

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

            await using var gasReadingListener = new GasReadingListener(readingCollector.SaveReadingIfValid);

            await gasReadingListener.StartListeningForGasReadings();

            Thread.Sleep(20_000);
        }

        public static void PrintAveragesEveryMinute(ReadingCollector collector)
        {
            var timer = new Timer
            {
                Interval = 60_000,
                AutoReset = true,
            };

            timer.Elapsed += (sender, args) => PrintAverageReadingsForLastMinute(collector);
        }

        public static void PrintAverageReadingsForLastMinute(ReadingCollector collector)
        {
            var value = ReadingProcessing.ReadingProcessing.GetAverageReadings(
                collector.ReadingsTaken.Where(
                    reading => reading.TimeStamp.AddMinutes(1) > DateTime.UtcNow
                ).ToList()
            );
        } 
    }
}
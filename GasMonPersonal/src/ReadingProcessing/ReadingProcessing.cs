using System.Collections.Generic;
using System.Linq;
using GasMonPersonal.Models;

namespace GasMonPersonal.ReadingProcessing
{
    public static class ReadingProcessing
    {
        public static IEnumerable<AverageReading> GetAverageReadings(List<GasReading> readings)
        {
            return readings.GroupBy(
                reading => reading.LocationId,
                (locationId, localReadings) =>
                {
                    var localReadingList = localReadings.ToList();

                    return new AverageReading
                    {
                        LocationId = locationId,
                        IntervalStartTime = localReadingList.Select(reading => reading.TimeStamp)
                            .OrderBy(timeStamp => timeStamp)
                            .First(),
                        IntervalEndTime = localReadingList.Select(reading => reading.TimeStamp)
                            .OrderBy(timeStamp => timeStamp)
                            .Last(),
                        Value = localReadingList.Sum(reading => reading.Value) / localReadingList.Count,
                    };
                }
            );
        }
    }
}
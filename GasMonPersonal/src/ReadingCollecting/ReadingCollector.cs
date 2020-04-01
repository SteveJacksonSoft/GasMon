using System;
using System.Collections.Generic;
using System.Linq;
using GasMonPersonal.Models;

namespace GasMonPersonal.ReadingCollecting
{
    public class ReadingCollector
    {
        private readonly Func<GasReading, bool> _messageIsAccurate;

        private readonly IDuplicateChecker _duplicateChecker;

        private List<GasReading> readingsTaken = new List<GasReading>();

        public ReadingCollector(IDuplicateChecker duplicateChecker, Func<GasReading, bool> messageIsAccurate)
        {
            _messageIsAccurate = messageIsAccurate;
            _duplicateChecker = duplicateChecker;
        }

        public void SaveMessageIfValid(GasReading reading)
        {
            if (!_messageIsAccurate(reading)) return;
            if (_duplicateChecker.MessageIsDuplicate(reading)) return;

            _duplicateChecker.RecordReadingEventId(reading.EventId);
            
            PrintReading(reading);

            readingsTaken.Add(reading);
        }

        private static void PrintReading(GasReading reading)
        {
            Console.WriteLine($"{reading.EventId}: locationId = {reading.LocationId}; value = {reading.Value}");
        }
    }
}
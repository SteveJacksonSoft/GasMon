using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GasMonPersonal.Models;

namespace GasMonPersonal.ReadingCollecting
{
    public class DuplicateChecker : IDuplicateChecker, IDisposable
    {
        private const int ProcessRecordCleaningIntervalInMs = 10_000;
        
        private readonly Dictionary<string, DateTime> _processRecord = new Dictionary<string, DateTime>();

        private Timer _processRecordCleanupTimer;
        
        public DuplicateChecker()
        {
            CleanProcessRecordRegularly();
        }

        public void Dispose()
        {
            _processRecordCleanupTimer.Dispose();
        }

        public void RecordReadingEventId(string eventId)
        {
            _processRecord.Add(eventId, DateTime.Now);
        }

        public bool MessageIsDuplicate(GasReading reading)
        {
            return !_processRecord.ContainsKey(reading.EventId);
        }

        private void CleanProcessRecordRegularly()
        {
            _processRecordCleanupTimer = new Timer
            {
                Interval = ProcessRecordCleaningIntervalInMs,
                AutoReset = true,
            };

            _processRecordCleanupTimer.Elapsed += (sender, args) => CleanupProcessRecord();
            
            _processRecordCleanupTimer.Start();
        }

        private void CleanupProcessRecord()
        {
            var oldRecordKeys = _processRecord.Keys.Where(key => _processRecord[key].AddMinutes(5) < DateTime.UtcNow);
            
            foreach (var key in oldRecordKeys)
            {
                _processRecord.Remove(key);
            }
        }
    }
}
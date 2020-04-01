using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GasMonPersonal.Models;

namespace GasMonPersonal.MessageProcessing
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

        public void RecordMessage(GasMessage message)
        {
            _processRecord.Add(message.EventId, DateTime.Now);
        }

        public bool MessageIsDuplicate(GasMessage message)
        {
            return !_processRecord.ContainsKey(message.EventId);
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
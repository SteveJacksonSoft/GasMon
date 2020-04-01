using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GasMonPersonal.Models;

namespace GasMonPersonal.MessageProcessing
{
    public class MessageProcessor : IDisposable
    {
        private const int ProcessRecordCleaningIntervalInMs = 300_000; 
        
        private readonly List<Location> _accurateLocations;
        private readonly Dictionary<string, DateTime> _processRecord = new Dictionary<string, DateTime>();

        private Timer _processRecordCleanupTimer;

        public MessageProcessor(List<Location> accurateLocations)
        {
            _accurateLocations = accurateLocations;

            CleanProcessRecordRegularly();
        }

        public void Dispose()
        {
            _processRecordCleanupTimer.Dispose();
        }

        public void ProcessMessage(GasMessage message)
        {
            if (!MessageIsAccurate(message)) return;
            if (MessageIsAlreadyProcessed(message)) return;
            
            this._processRecord.Add(message.EventId, DateTime.Now);
            Console.WriteLine($"{message.EventId}: locationId = {message.LocationId}; value = {message.Value}");
        }

        private bool MessageIsAccurate(GasMessage message)
        {
            return _accurateLocations.Any(location => location.Id == message.LocationId);
        }

        private bool MessageIsAlreadyProcessed(GasMessage message)
        {
            return this._processRecord.ContainsKey(message.EventId);
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
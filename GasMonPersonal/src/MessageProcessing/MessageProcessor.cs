using System;
using GasMonPersonal.Models;

namespace GasMonPersonal.MessageProcessing
{
    public class MessageProcessor
    {
        private readonly Func<GasMessage, bool> _messageIsAccurate;

        private readonly IDuplicateChecker _duplicateChecker;

        public MessageProcessor(IDuplicateChecker duplicateChecker, Func<GasMessage, bool> messageIsAccurate)
        {
            _messageIsAccurate = messageIsAccurate;
            _duplicateChecker = duplicateChecker;
        }

        public void ProcessMessage(GasMessage message)
        {
            if (!_messageIsAccurate(message)) return;
            if (_duplicateChecker.MessageIsDuplicate(message)) return;

            _duplicateChecker.RecordMessage(message);
            
            Console.WriteLine($"{message.EventId}: locationId = {message.LocationId}; value = {message.Value}");
        }
    }
}
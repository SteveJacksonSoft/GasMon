using GasMonPersonal.Models;

namespace GasMonPersonal.ReadingCollecting
{
    public interface IDuplicateChecker
    {
        public void RecordReadingEventId(string eventId);

        public bool MessageIsDuplicate(GasReading reading);
    }
}
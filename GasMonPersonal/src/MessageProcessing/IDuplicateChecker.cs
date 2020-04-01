using GasMonPersonal.Models;

namespace GasMonPersonal.MessageProcessing
{
    public interface IDuplicateChecker
    {
        public void RecordMessage(GasMessage message);

        public bool MessageIsDuplicate(GasMessage message);
    }
}
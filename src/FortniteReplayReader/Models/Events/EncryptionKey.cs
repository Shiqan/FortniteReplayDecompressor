using FortniteReplayReader.Models.Events;

namespace FortniteReplayReader.Models.Events
{
    public class EncryptionKey : BaseEvent
    {
        public string Key { get; set; }
    }
}
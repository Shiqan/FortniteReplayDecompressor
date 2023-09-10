using FortniteReplayReader.Models.NetFieldExports;

namespace FortniteReplayReader.Models;

public class QueuedPlayerPawn
{
    public uint ChannelId { get; set; }
    public PlayerPawn PlayerPawn { get; set; }
}
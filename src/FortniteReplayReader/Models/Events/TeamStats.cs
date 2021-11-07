namespace FortniteReplayReader.Models.Events;

public class TeamStats : BaseEvent
{
    public uint Unknown { get; set; }
    public uint Position { get; set; }
    public uint TotalPlayers { get; set; }
}

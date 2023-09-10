using Unreal.Core.Models;

namespace FortniteReplayReader.Models;

public class SupplyDrop
{
    public SupplyDrop()
    {

    }

    public SupplyDrop(uint channelIndex, NetFieldExports.SupplyDrop drop)
    {
        Id = channelIndex;
        FallHeight = drop.FallHeight;
        FallSpeed = drop.FallSpeed;
    }

    public uint Id { get; set; }
    public bool HasSpawnedPickups { get; set; }
    public bool Looted { get; set; }
    public float? LootedTime { get; set; }
    public double? LootedTimeDouble { get; set; }
    public bool BalloonPopped { get; set; }
    public float? BalloonPoppedTime { get; set; }
    public double? BalloonPoppedTimeDouble { get; set; }
    public float FallSpeed { get; set; }
    public FVector LandingLocation { get; set; }
    public float FallHeight { get; set; }
}

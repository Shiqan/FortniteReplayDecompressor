using FortniteReplayReader.Models.NetFieldExports;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models;

public class SafeZone
{
    public SafeZone()
    {

    }

    public SafeZone(SafeZoneIndicator safeZone)
    {
        Radius = safeZone.Radius;
        StartShrinkTime = safeZone.SafeZoneStartShrinkTime;
        FinishShrinkTime = safeZone.SafeZoneFinishShrinkTime;
        LastRadius = safeZone.LastRadius;
        LastCenter = safeZone.LastCenter;
        NextRadius = safeZone.NextRadius;
        NextCenter = safeZone.NextCenter;
        NextNextRadius = safeZone.NextNextRadius;
        NextNextCenter = safeZone.NextNextCenter;
    }

    public float Radius { get; set; }
    public float StartShrinkTime { get; set; }
    public float FinishShrinkTime { get; set; }

    public float LastRadius { get; set; }
    public FVector LastCenter { get; set; }

    public float NextRadius { get; set; }
    public FVector NextCenter { get; set; }

    public float NextNextRadius { get; set; }
    public FVector NextNextCenter { get; set; }
}

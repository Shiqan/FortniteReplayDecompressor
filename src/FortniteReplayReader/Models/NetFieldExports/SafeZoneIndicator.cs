using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/SafeZone/SafeZoneIndicator.SafeZoneIndicator_C")]
    public class SafeZoneIndicator : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("LastRadius", RepLayoutCmdType.PropertyFloat, 15, "LastRadius", "float", 32)]
        public float? LastRadius { get; set; } //Type: float Bits: 32

        [NetFieldExport("NextRadius", RepLayoutCmdType.PropertyFloat, 16, "NextRadius", "float", 32)]
        public float? NextRadius { get; set; } //Type: float Bits: 32

        [NetFieldExport("NextNextRadius", RepLayoutCmdType.PropertyFloat, 17, "NextNextRadius", "float", 32)]
        public float? NextNextRadius { get; set; } //Type: float Bits: 32

        [NetFieldExport("LastCenter", RepLayoutCmdType.PropertyVector100, 18, "LastCenter", "FVector_NetQuantize100", 74)]
        public FVector LastCenter { get; set; } //Type: FVector_NetQuantize100 Bits: 74

        [NetFieldExport("NextCenter", RepLayoutCmdType.PropertyVector100, 19, "NextCenter", "FVector_NetQuantize100", 74)]
        public FVector NextCenter { get; set; } //Type: FVector_NetQuantize100 Bits: 74

        [NetFieldExport("NextNextCenter", RepLayoutCmdType.PropertyVector100, 20, "NextNextCenter", "FVector_NetQuantize100", 74)]
        public FVector NextNextCenter { get; set; } //Type: FVector_NetQuantize100 Bits: 74

        [NetFieldExport("SafeZoneStartShrinkTime", RepLayoutCmdType.PropertyFloat, 21, "SafeZoneStartShrinkTime", "float", 32)]
        public float? SafeZoneStartShrinkTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("SafeZoneFinishShrinkTime", RepLayoutCmdType.PropertyFloat, 22, "SafeZoneFinishShrinkTime", "float", 32)]
        public float? SafeZoneFinishShrinkTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("bPausedForPreview", RepLayoutCmdType.PropertyBool, 25, "bPausedForPreview", "", 1)]
        public bool? bPausedForPreview { get; set; } //Type:  Bits: 1

        [NetFieldExport("MegaStormDelayTimeBeforeDestruction", RepLayoutCmdType.PropertyFloat, 27, "MegaStormDelayTimeBeforeDestruction", "float", 32)]
        public float? MegaStormDelayTimeBeforeDestruction { get; set; } //Type: float Bits: 32

        [NetFieldExport("Radius", RepLayoutCmdType.PropertyFloat, 31, "Radius", "float", 32)]
        public float? Radius { get; set; } //Type:  Bits: 32

    }
}
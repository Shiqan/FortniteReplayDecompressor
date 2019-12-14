using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/SupplyDrops/AthenaSupplyDrop.AthenaSupplyDrop_C")]
    public class SupplyDrop : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement, 5, "ReplicatedMovement", "FRepMovement", 109)]
        public FRepMovement ReplicatedMovement { get; set; } //Type: FRepMovement Bits: 109

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool, 25, "bDestroyed", "uint8", 1)]
        public bool? bDestroyed { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bEditorPlaced", RepLayoutCmdType.PropertyBool, 26, "bEditorPlaced", "uint8", 1)]
        public bool? bEditorPlaced { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool, 33, "bInstantDeath", "uint8", 1)]
        public bool? bInstantDeath { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bHasSpawnedPickups", RepLayoutCmdType.PropertyBool, 38, "bHasSpawnedPickups", "bool", 1)]
        public bool? bHasSpawnedPickups { get; set; } //Type: bool Bits: 1

        [NetFieldExport("Opened", RepLayoutCmdType.PropertyBool, 40, "Opened", "bool", 1)]
        public bool? Opened { get; set; } //Type: bool Bits: 1

        [NetFieldExport("BalloonPopped", RepLayoutCmdType.PropertyBool, 41, "BalloonPopped", "bool", 1)]
        public bool? BalloonPopped { get; set; } //Type: bool Bits: 1

        [NetFieldExport("FallSpeed", RepLayoutCmdType.PropertyFloat, 42, "FallSpeed", "float", 32)]
        public float? FallSpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("LandingLocation", RepLayoutCmdType.Property, 43, "LandingLocation", "FVector", 96)]
        public FVector LandingLocation { get; set; } //Type: FVector Bits: 96

        [NetFieldExport("FallHeight", RepLayoutCmdType.PropertyFloat, 44, "FallHeight", "float", 32)]
        public float? FallHeight { get; set; } //Type: float Bits: 32

    }
}
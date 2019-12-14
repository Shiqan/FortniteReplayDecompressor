using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/SupplyDrops/Llama/AthenaSupplyDrop_Llama.AthenaSupplyDrop_Llama_C")]
    public class SupplyDropLlama : INetFieldExportGroup
    {
        [NetFieldExport("bHidden", RepLayoutCmdType.PropertyBool, 0, "bHidden", "uint8", 1)]
        public bool? bHidden { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement, 5, "ReplicatedMovement", "FRepMovement", 79)]
        public FRepMovement ReplicatedMovement { get; set; } //Type: FRepMovement Bits: 79

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool, 25, "bDestroyed", "uint8", 1)]
        public bool? bDestroyed { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bEditorPlaced", RepLayoutCmdType.PropertyBool, 26, "bEditorPlaced", "uint8", 1)]
        public bool? bEditorPlaced { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool, 31, "bInstantDeath", "", 1)]
        public bool? bInstantDeath { get; set; } //Type:  Bits: 1

        [NetFieldExport("bHasSpawnedPickups", RepLayoutCmdType.PropertyBool, 38, "bHasSpawnedPickups", "bool", 1)]
        public bool? bHasSpawnedPickups { get; set; } //Type: bool Bits: 1

        [NetFieldExport("Looted", RepLayoutCmdType.PropertyBool, 40, "Looted", "bool", 1)]
        public bool? Looted { get; set; } //Type: bool Bits: 1

        [NetFieldExport("FinalDestination", RepLayoutCmdType.PropertyVector, 41, "FinalDestination", "FVector", 96)]
        public FVector FinalDestination { get; set; } //Type: FVector Bits: 96

    }
}
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Game/Athena/Deimos/Spawners/RiftSpawners/AthenaSupplyDrop_DeimosSpawner.AthenaSupplyDrop_DeimosSpawner_C", minimalParseMode: ParseMode.Ignore)]
public class SupplyDropDeimosSpawner : INetFieldExportGroup
{
    [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement)]
    public FRepMovement ReplicatedMovement { get; set; }

    [NetFieldExport("bEditorPlaced", RepLayoutCmdType.PropertyBool)]
    public bool? bEditorPlaced { get; set; }
}
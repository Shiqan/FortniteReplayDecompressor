using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Script/FortniteGame.SpawnMachineRepData", minimalParseMode: ParseMode.Full)]
public class SpawnMachineRepData : INetFieldExportGroup
{
    [NetFieldExport("Location", RepLayoutCmdType.PropertyVector)]
    public FVector Location { get; set; }

    [NetFieldExport("SpawnMachineState", RepLayoutCmdType.Enum)]
    public int SpawnMachineState { get; set; }

    [NetFieldExport("SpawnMachineCooldownStartTime", RepLayoutCmdType.PropertyFloat)]
    public float SpawnMachineCooldownStartTime { get; set; }

    [NetFieldExport("SpawnMachineCooldownEndTime", RepLayoutCmdType.PropertyFloat)]
    public float SpawnMachineCooldownEndTime { get; set; }

    [NetFieldExport("SpawnMachineRepDataHandle", RepLayoutCmdType.PropertyInt)]
    public int SpawnMachineRepDataHandle { get; set; }
}

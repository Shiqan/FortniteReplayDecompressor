using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Game/Spectating/BP_ReplayPC_Athena.BP_ReplayPC_Athena_C", minimalParseMode: ParseMode.Minimal)]
[PlayerController("BP_ReplayPC_Athena_C")]
public class ReplayPC : INetFieldExportGroup
{
    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public int? Role { get; set; }

    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int? RemoteRole { get; set; }

    [NetFieldExport("PlayerState", RepLayoutCmdType.PropertyObject)]
    public uint PlayerState { get; set; }

    [NetFieldExport("SpawnLocation", RepLayoutCmdType.PropertyVector)]
    public FVector SpawnLocation { get; set; }
}

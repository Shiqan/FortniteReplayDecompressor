using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.RPC;

[NetFieldExportGroup("/Script/FortniteGame.FortPlayerPawnAthena:FastSharedReplication", minimalParseMode: ParseMode.Debug)]
public class FastSharedReplication : INetFieldExportGroup
{
    [NetFieldExport("SharedRepMovement", RepLayoutCmdType.RepMovement)]
    public FRepMovement SharedRepMovement { get; set; }
}

using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.RPC;

[NetFieldExportGroup("/Script/FortniteGame.FortPlayerStateAthena:Client_OnNewLevel", minimalParseMode: ParseMode.Debug)]
public class OnNewLevel : INetFieldExportGroup
{
    [NetFieldExport("NewLevel", RepLayoutCmdType.PropertyInt)]
    public int NewLevel { get; set; }
}

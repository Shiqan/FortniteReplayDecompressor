using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportClassNetCache("FortTeamPrivateInfo_ClassNetCache", minimalParseMode: ParseMode.Debug)]
public class FortTeamPrivateInfoCache
{
    [NetFieldExportRPC("LatentTeamRepData", "/Script/FortniteGame.FortTeamPrivateInfo")]
    public object LatentTeamRepData { get; set; }

    [NetFieldExportRPC("RepData", "/Script/FortniteGame.FortTeamPrivateInfo")]
    public object RepData { get; set; }
}

[NetFieldExportGroup("/Script/FortniteGame.FortTeamPrivateInfo", minimalParseMode: ParseMode.Debug)]
public class FortTeamPrivateInfo : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int? RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public int? Role { get; set; }

    [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject)]
    public ActorGuid Owner { get; set; }

    [NetFieldExport("Value", RepLayoutCmdType.PropertyFloat)]
    public float? Value { get; set; }

    [NetFieldExport("PlayerId", RepLayoutCmdType.PropertyNetId)]
    public string PlayerId { get; set; }

    [NetFieldExport("PlayerID", RepLayoutCmdType.PropertyNetId)]
    public string PlayerID { get; set; }

    [NetFieldExport("PlayerState", RepLayoutCmdType.Property)]
    public ActorGuid PlayerState { get; set; }

    [NetFieldExport("LastRepLocation", RepLayoutCmdType.PropertyVector100)]
    public FVector LastRepLocation { get; set; }

    [NetFieldExport("LastRepYaw", RepLayoutCmdType.PropertyFloat)]
    public float? LastRepYaw { get; set; }

    [NetFieldExport("PawnStateMask", RepLayoutCmdType.Enum)]
    public int? PawnStateMask { get; set; }
}
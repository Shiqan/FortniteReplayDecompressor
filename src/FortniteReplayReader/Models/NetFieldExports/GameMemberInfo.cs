using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

/// <summary>
/// GameMemberInfo gives for each real player their <see cref="TeamIndex"></see> and <see cref="SquadId"/>.
/// However, <see cref="FortPlayerState"/> also contains this information, so no need to track this group.
/// </summary>
[NetFieldExportGroup("/Script/FortniteGame.GameMemberInfo", minimalParseMode: ParseMode.Ignore)]
public class GameMemberInfo : INetFieldExportGroup
{
    [NetFieldExport("SquadId", RepLayoutCmdType.PropertyByte)]
    public byte SquadId { get; set; }

    [NetFieldExport("TeamIndex", RepLayoutCmdType.Enum)]
    public int TeamIndex { get; set; }

    [NetFieldExport("MemberUniqueId", RepLayoutCmdType.PropertyNetId)]
    public string MemberUniqueId { get; set; }
}

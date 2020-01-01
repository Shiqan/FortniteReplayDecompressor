using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.GameMemberInfo")]
    public class GameMemberInfo : INetFieldExportGroup
    {
        [NetFieldExport("SquadId", RepLayoutCmdType.PropertyName)]
        public string SquadId { get; set; }

        [NetFieldExport("TeamIndex", RepLayoutCmdType.Enum)]
        public int TeamIndex { get; set; }

        [NetFieldExport("MemberUniqueId", RepLayoutCmdType.PropertyNetId)]
        public string MemberUniqueId { get; set; }
    }
}

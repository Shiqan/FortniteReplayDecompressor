using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.FortInventory")]
    public class FortInventory : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
        public object RemoteRole { get; set; }

        [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject)]
        public uint? Owner { get; set; }

        [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
        public object Role { get; set; }

        [NetFieldExport("ReplayPawn", RepLayoutCmdType.PropertyUInt32)]
        public uint? ReplayPawn { get; set; }
    }
}
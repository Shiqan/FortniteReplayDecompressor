using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.FortInventory")]
    public class FortInventory : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject, 12, "Owner", "AActor*", 8)]
        public uint? Owner { get; set; } //Type: AActor* Bits: 8

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("ReplayPawn", RepLayoutCmdType.PropertyUInt32, 49, "ReplayPawn", "", 16)]
        public uint? ReplayPawn { get; set; } //Type:  Bits: 16

    }
}
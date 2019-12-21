using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportSubGroup("/Script/FortniteGame.FortPickupAthena")]
    public class FortItemEntryStateValue
    {
        [NetFieldExport("StateType", RepLayoutCmdType.Enum, 34, "StateType", "", 0)]
        public int StateType { get; set; } //Type:  Bits: 0

        [NetFieldExport("IntValue", RepLayoutCmdType.PropertyInt, 35, "IntValue", "int32", 0)]
        public int? IntValue { get; set; } //Type: int32 Bits: 0

        [NetFieldExport("NameValue", RepLayoutCmdType.Property, 36, "NameValue", "FName", 0)]
        public FName NameValue { get; set; } //Type: FName Bits: 0
    }
}

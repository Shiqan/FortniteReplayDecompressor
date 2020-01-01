using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.SpawnMachineRepData")]
    public class SpawnMachineRepData : INetFieldExportGroup
    {
        [NetFieldExport("Location", RepLayoutCmdType.PropertyVector100)]
        public DebuggingObject Location { get; set; }

        [NetFieldExport("SpawnMachineState", RepLayoutCmdType.Property)]
        public DebuggingObject SpawnMachineState { get; set; }

        [NetFieldExport("SpawnMachineCooldownStartTime", RepLayoutCmdType.PropertyUInt32)]
        public DebuggingObject SpawnMachineCooldownStartTime { get; set; }

        [NetFieldExport("SpawnMachineCooldownEndTime", RepLayoutCmdType.PropertyUInt32)]
        public DebuggingObject SpawnMachineCooldownEndTime { get; set; }

        [NetFieldExport("SpawnMachineRepDataHandle", RepLayoutCmdType.PropertyUInt32)]
        public DebuggingObject SpawnMachineRepDataHandle { get; set; }
    }
}

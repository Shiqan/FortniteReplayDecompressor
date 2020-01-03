using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/DrivableVehicles/Meatball/Meatball_Large/MeatballVehicle_L.MeatballVehicle_L_C", minimalParseMode: ParseMode.Debug)]
    public class Boat : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
        public int RemoteRole { get; set; }

        [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
        public int Role { get; set; }

        [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
        public uint Instigator { get; set; }

        [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement)]
        public FRepMovement ReplicatedMovement { get; set; }

        [NetFieldExport("Location", RepLayoutCmdType.PropertyVector100)]
        public FVector Location { get; set; }

        [NetFieldExport("UpVector", RepLayoutCmdType.Property)]
        public DebuggingObject UpVector { get; set; }

        [NetFieldExport("ForwardVector", RepLayoutCmdType.Property)]
        public DebuggingObject ForwardVector { get; set; }

        [NetFieldExport("SurfaceTypeVehicleOn", RepLayoutCmdType.Property)]
        public DebuggingObject SurfaceTypeVehicleOn { get; set; }
    }
}
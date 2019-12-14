using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/Aircraft/AthenaAircraft.AthenaAircraft_C")]
    public class Aircraft : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("JumpFlashCount", RepLayoutCmdType.PropertyInt, 15, "JumpFlashCount", "int32", 32)]
        public int? JumpFlashCount { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("FlightStartLocation", RepLayoutCmdType.PropertyVector100, 16, "FlightStartLocation", "FVector_NetQuantize100", 80)]
        public FVector FlightStartLocation { get; set; } //Type: FVector_NetQuantize100 Bits: 80

        [NetFieldExport("FlightStartRotation", RepLayoutCmdType.PropertyRotator, 17, "FlightStartRotation", "FRotator", 19)]
        public FRotator FlightStartRotation { get; set; } //Type: FRotator Bits: 19

        [NetFieldExport("FlightSpeed", RepLayoutCmdType.PropertyFloat, 18, "FlightSpeed", "float", 32)]
        public float? FlightSpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("TimeTillFlightEnd", RepLayoutCmdType.PropertyFloat, 19, "TimeTillFlightEnd", "float", 32)]
        public float? TimeTillFlightEnd { get; set; } //Type: float Bits: 32

        [NetFieldExport("TimeTillDropStart", RepLayoutCmdType.PropertyFloat, 20, "TimeTillDropStart", "float", 32)]
        public float? TimeTillDropStart { get; set; } //Type: float Bits: 32

        [NetFieldExport("TimeTillDropEnd", RepLayoutCmdType.PropertyFloat, 21, "TimeTillDropEnd", "float", 32)]
        public float? TimeTillDropEnd { get; set; } //Type: float Bits: 32

        [NetFieldExport("FlightStartTime", RepLayoutCmdType.PropertyFloat, 22, "FlightStartTime", "float", 32)]
        public float? FlightStartTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("FlightEndTime", RepLayoutCmdType.PropertyFloat, 23, "FlightEndTime", "float", 32)]
        public float? FlightEndTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("DropStartTime", RepLayoutCmdType.PropertyFloat, 24, "DropStartTime", "float", 32)]
        public float? DropStartTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("DropEndTime", RepLayoutCmdType.PropertyFloat, 25, "DropEndTime", "float", 32)]
        public float? DropEndTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("ReplicatedFlightTimestamp", RepLayoutCmdType.PropertyFloat, 26, "ReplicatedFlightTimestamp", "float", 32)]
        public float? ReplicatedFlightTimestamp { get; set; } //Type: float Bits: 32

        [NetFieldExport("AircraftIndex", RepLayoutCmdType.PropertyUInt32, 27, "AircraftIndex", "", 32)]
        public uint? AircraftIndex { get; set; } //Type:  Bits: 32

    }
}
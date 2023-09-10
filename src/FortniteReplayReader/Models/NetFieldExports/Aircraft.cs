using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Game/Athena/Aircraft/AthenaAircraft.AthenaAircraft_C", minimalParseMode: ParseMode.Ignore)]
public class Aircraft : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public object RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("JumpFlashCount", RepLayoutCmdType.PropertyInt)]
    public int JumpFlashCount { get; set; }

    [NetFieldExport("FlightStartLocation", RepLayoutCmdType.PropertyVector100)]
    public FVector FlightStartLocation { get; set; }

    [NetFieldExport("FlightStartRotation", RepLayoutCmdType.PropertyRotator)]
    public FRotator FlightStartRotation { get; set; }

    [NetFieldExport("FlightSpeed", RepLayoutCmdType.PropertyFloat)]
    public float FlightSpeed { get; set; }

    [NetFieldExport("TimeTillFlightEnd", RepLayoutCmdType.PropertyFloat)]
    public float TimeTillFlightEnd { get; set; }

    [NetFieldExport("TimeTillDropStart", RepLayoutCmdType.PropertyFloat)]
    public float TimeTillDropStart { get; set; }

    [NetFieldExport("TimeTillDropEnd", RepLayoutCmdType.PropertyFloat)]
    public float TimeTillDropEnd { get; set; }

    [NetFieldExport("FlightStartTime", RepLayoutCmdType.PropertyFloat)]
    public float FlightStartTime { get; set; }

    [NetFieldExport("FlightEndTime", RepLayoutCmdType.PropertyFloat)]
    public float FlightEndTime { get; set; }

    [NetFieldExport("DropStartTime", RepLayoutCmdType.PropertyFloat)]
    public float DropStartTime { get; set; }

    [NetFieldExport("DropEndTime", RepLayoutCmdType.PropertyFloat)]
    public float DropEndTime { get; set; }

    [NetFieldExport("ReplicatedFlightTimestamp", RepLayoutCmdType.PropertyFloat)]
    public float ReplicatedFlightTimestamp { get; set; }

    [NetFieldExport("AircraftIndex", RepLayoutCmdType.PropertyUInt32)]
    public uint AircraftIndex { get; set; }
}
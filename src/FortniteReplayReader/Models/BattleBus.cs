using FortniteReplayReader.Models.NetFieldExports;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models;

public class BattleBus
{
    public BattleBus()
    {

    }

    public BattleBus(Aircraft aircraft)
    {
        AircraftIndex = aircraft.AircraftIndex;
        FlightStartLocation = aircraft.FlightStartLocation;
        FlightStartRotation = aircraft.FlightStartRotation;
        FlightSpeed = aircraft.FlightSpeed;
        TimeTillFlightEnd = aircraft.TimeTillFlightEnd;
        TimeTillDropStart = aircraft.TimeTillDropStart;
        TimeTillDropEnd = aircraft.TimeTillDropEnd;
        ReplicatedFlightTimestamp = aircraft.ReplicatedFlightTimestamp;
    }

    public uint AircraftIndex { get; set; }
    public string Skin { get; set; }
    public FVector FlightStartLocation { get; set; }
    public FRotator FlightStartRotation { get; set; }
    public float FlightSpeed { get; set; }
    public float TimeTillFlightEnd { get; set; }
    public float TimeTillDropStart { get; set; }
    public float TimeTillDropEnd { get; set; }
    public float ReplicatedFlightTimestamp { get; set; }
}

using FortniteReplayReader.Models.NetFieldExports.Weapons;
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Vehicles;

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
    [RepMovement(
        locationQuantizationLevel: VectorQuantization.RoundWholeNumber,
        rotationQuantizationLevel: RotatorQuantization.ShortComponents,
        velocityQuantizationLevel: VectorQuantization.RoundTwoDecimals)]
    public FRepMovement ReplicatedMovement { get; set; }

    [NetFieldExport("Location", RepLayoutCmdType.PropertyVector)]
    public FVector Location { get; set; }

    [NetFieldExport("UpVector", RepLayoutCmdType.PropertyVector)]
    public FVector UpVector { get; set; }

    [NetFieldExport("ForwardVector", RepLayoutCmdType.PropertyVector)]
    public FVector ForwardVector { get; set; }

    [NetFieldExport("SurfaceTypeVehicleOn", RepLayoutCmdType.Enum)]
    public int SurfaceTypeVehicleOn { get; set; }

    [NetFieldExport("InitialOverlappingVehicles", RepLayoutCmdType.Property)]
    public DebuggingObject InitialOverlappingVehicles { get; set; }
}

[NetFieldExportClassNetCache("B_Prj_Meatball_Missile_C_ClassNetCache", minimalParseMode: ParseMode.Debug)]
public class BoatMissleClassNetCache : BaseExplosion
{
}
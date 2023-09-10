using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/Assault/B_Minigun_Athena.B_Minigun_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Minigun : BaseWeapon
{
    [NetFieldExport("ChargeStatusPack", RepLayoutCmdType.Property)]
    public DebuggingObject ChargeStatusPack { get; set; }

    [NetFieldExport("CurrentSpinAudioComponent", RepLayoutCmdType.Property)]
    public NetworkGUID CurrentSpinAudioComponent { get; set; }

    [NetFieldExport("bIsChargingWeapon", RepLayoutCmdType.PropertyBool)]
    public bool? bIsChargingWeapon { get; set; }

    [NetFieldExport("SpinVolumeMultiplier", RepLayoutCmdType.PropertyFloat)]
    public float? SpinVolumeMultiplier { get; set; }

    [NetFieldExport("bPlayedSpinUpAudio", RepLayoutCmdType.PropertyBool)]
    public bool? bPlayedSpinUpAudio { get; set; }

    [NetFieldExport("bPlayedSpinDownAudio", RepLayoutCmdType.PropertyBool)]
    public bool? bPlayedSpinDownAudio { get; set; }

    [NetFieldExport("OverheatValue", RepLayoutCmdType.PropertyUInt32)]
    public uint? OverheatValue { get; set; }

    [NetFieldExport("OverheatState", RepLayoutCmdType.Enum)]
    public int? OverheatState { get; set; }
}

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/Assault/B_Assault_LMG_SAW_Athena.B_Assault_LMG_SAW_Athena_C", minimalParseMode: ParseMode.Debug)]
public class LightMachineGun : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/Assault/B_Assault_MidasDrum_Athena.B_Assault_MidasDrum_Athena_C", minimalParseMode: ParseMode.Debug)]
public class MidasDrumGun : BaseWeapon
{
}

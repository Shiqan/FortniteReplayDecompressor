using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Athena/Items/Consumables/FloppingRabbit/B_FloppingRabbit_Weap_Athena.B_FloppingRabbit_Weap_Athena_C", minimalParseMode: ParseMode.Debug)]
public class FishingRod : BaseWeapon
{
    [NetFieldExport("Projectile", RepLayoutCmdType.Property)]
    public NetworkGUID Projectile { get; set; }

    [NetFieldExport("Wire", RepLayoutCmdType.Property)]
    public NetworkGUID Wire { get; set; }

    [NetFieldExport("HideBobber", RepLayoutCmdType.PropertyBool)]
    public bool? HideBobber { get; set; }

    [NetFieldExport("OneHandGrip", RepLayoutCmdType.PropertyBool)]
    public bool? OneHandGrip { get; set; }
}

[NetFieldExportGroup("/Game/Athena/Items/Consumables/FloppingRabbit/B_Athena_FloppingRabbit_Wire.B_Athena_FloppingRabbit_Wire_C", minimalParseMode: ParseMode.Debug)]
public class FishingRodWire : BaseWeapon
{
    [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement)]
    [RepMovement(
        locationQuantizationLevel: VectorQuantization.RoundWholeNumber,
        rotationQuantizationLevel: RotatorQuantization.ByteComponents,
        velocityQuantizationLevel: VectorQuantization.RoundWholeNumber)]
    public FRepMovement ReplicatedMovement { get; set; }

    [NetFieldExport("Projectile", RepLayoutCmdType.Property)]
    public NetworkGUID Projectile { get; set; }

    [NetFieldExport("Projectile Actor", RepLayoutCmdType.Property)]
    public NetworkGUID ProjectileActor { get; set; }

    [NetFieldExport("PlayerPawn", RepLayoutCmdType.Property)]
    public NetworkGUID PlayerPawn { get; set; }

    [NetFieldExport("CatchParticleOn", RepLayoutCmdType.PropertyBool)]
    public bool? CatchParticleOn { get; set; }

    [NetFieldExport("Weapon", RepLayoutCmdType.Property)]
    public NetworkGUID Weapon { get; set; }
}

[NetFieldExportGroup("/Game/Athena/Items/Consumables/HappyGhost/B_HappyGhost_Athena.B_HappyGhost_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Harpoon : BaseWeapon
{
    [NetFieldExport("HideProj", RepLayoutCmdType.PropertyBool)]
    public bool? HideProj { get; set; }
}

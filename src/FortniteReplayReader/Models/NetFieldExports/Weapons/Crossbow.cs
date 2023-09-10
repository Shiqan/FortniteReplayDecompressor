using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_Crossbows/Blueprints/B_TnTinaBow_Athena.B_TnTinaBow_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Crossbow : BaseWeapon
{
    [NetFieldExport("ChargeStatusPack", RepLayoutCmdType.PropertyUInt16)]
    public ushort? ChargeStatusPack { get; set; }

    [NetFieldExport("bIsChargingWeapon", RepLayoutCmdType.PropertyBool)]
    public bool? bIsChargingWeapon { get; set; }
}

[NetFieldExportGroup("/Game/Weapons/FORT_Crossbows/Blueprints/B_Valentine_Crossbow_Athena.B_Valentine_Crossbow_Athena_C", minimalParseMode: ParseMode.Debug)]
public class ValentineCrossbow : Crossbow
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Crossbows/Blueprints/B_DemonHunter_Crossbow_Athena.B_DemonHunter_Crossbow_Athena_C", minimalParseMode: ParseMode.Debug)]
public class DemonHunterCrossbow : Crossbow
{
}

// /Game/Weapons/FORT_Crossbows/Blueprints/B_Prj_Arrow_ExplodeOnImpact_Athena_TnTina.B_Prj_Arrow_ExplodeOnImpact_Athena_TnTina_C

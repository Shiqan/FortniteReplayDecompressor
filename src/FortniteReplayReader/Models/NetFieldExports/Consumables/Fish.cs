using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Consumables;

public abstract class Fish : BaseConsumable
{
}

[NetFieldExportGroup("/Game/Athena/Items/Consumables/Flopper/Small/B_FlopperSmall_Weap_Athena.B_FlopperSmall_Weap_Athena_C", minimalParseMode: ParseMode.Debug)]
public class SmallFry : Fish
{
}

[NetFieldExportGroup("/Game/Athena/Items/Consumables/Flopper/B_Flopper_Weap_Athena.B_Flopper_Weap_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Flopper : Fish
{
}

[NetFieldExportGroup("/Game/Athena/Items/Consumables/Flopper/Effective/B_EffectiveFlopper_Weap_Athena.B_EffectiveFlopper_Weap_Athena_C", minimalParseMode: ParseMode.Debug)]
public class SlurpFish : Fish
{
}

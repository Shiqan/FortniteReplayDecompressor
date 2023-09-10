using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Consumables;

public abstract class Health : BaseConsumable
{
}

[NetFieldExportGroup("/Game/Abilities/Player/Generic/UtilityItems/B_ConsumableSmall_Bandages_Athena.B_ConsumableSmall_Bandages_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Bandages : Health
{
}

[NetFieldExportGroup("/Game/Abilities/Player/Generic/UtilityItems/B_ConsumableSmall_Medkit_Athena.B_ConsumableSmall_Medkit_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Medkit : Health
{
}

using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Consumables;

public abstract class Shield : BaseConsumable
{
}

[NetFieldExportGroup("/Game/Abilities/Player/Generic/UtilityItems/B_ConsumableSmall_MiniShield_Athena.B_ConsumableSmall_MiniShield_Athena_C", minimalParseMode: ParseMode.Debug)]
public class MiniShield : Shield
{
}

[NetFieldExportGroup("/Game/Abilities/Player/Generic/UtilityItems/B_ConsumableSmall_HalfShield_Athena.B_ConsumableSmall_HalfShield_Athena_C", minimalParseMode: ParseMode.Debug)]
public class HalfShield : Shield
{
}

[NetFieldExportGroup("/Game/Abilities/Player/Generic/UtilityItems/B_UtilityItem_Generic_Athena.B_UtilityItem_Generic_Athena_C", minimalParseMode: ParseMode.Debug)]
public class ChugJug : Shield
{
}

using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.AI;

[NetFieldExportGroup("/Game/Athena/AI/Phoebe/BP_PlayerPawn_Athena_Phoebe.BP_PlayerPawn_Athena_Phoebe_C", minimalParseMode: ParseMode.Full)]
public class PhoebePlayerPawn : PlayerPawn
{
}

[NetFieldExportGroup("/Game/Athena/AI/MANG/BP_MangPlayerPawn_Default.BP_MangPlayerPawn_Default_C", minimalParseMode: ParseMode.Full)]
public class MangPlayerPawn : PlayerPawn
{
    [NetFieldExport("AlertLevel", RepLayoutCmdType.Enum)]
    public int? AlertLevel { get; set; }

    [NetFieldExport("bIsStaggered", RepLayoutCmdType.PropertyBool)]
    public bool? bIsStaggered { get; set; }

    [NetFieldExport("StealthMeterTarget_4_A61BD65840C63E1798329EAE84F4B5C7", RepLayoutCmdType.PropertyFloat)]
    public float? StealthMeterTarget { get; set; }

    [NetFieldExport("StealthMeterTargetTime_5_627E99734167FD2903748490D5FC2A57", RepLayoutCmdType.PropertyFloat)]
    public float? StealthMeterTargetTime { get; set; }
}

[NetFieldExportGroup("/Game/Athena/AI/MANG/BP_MangPlayerPawn_Boss_AdventureGirl.BP_MangPlayerPawn_Boss_AdventureGirl_C", minimalParseMode: ParseMode.Full)]
public class MangBossPlayerPawn : MangPlayerPawn
{
}

[NetFieldExportGroup("/Game/Athena/AI/MANG/MangDataTracker.MangDataTracker_C", minimalParseMode: ParseMode.Full)]
public class MangDataTracker : INetFieldExportGroup
{
    [NetFieldExport("BotPawn", RepLayoutCmdType.Property)]
    public ActorGuid BotPawn { get; set; }

    [NetFieldExport("CurrentBotAlertLevel", RepLayoutCmdType.Enum)]
    public int? CurrentBotAlertLevel { get; set; }
}

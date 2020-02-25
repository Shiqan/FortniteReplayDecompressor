using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.AI
{
    [NetFieldExportGroup("/Game/Athena/AI/Phoebe/BP_PlayerPawn_Athena_Phoebe.BP_PlayerPawn_Athena_Phoebe_C", minimalParseMode: ParseMode.Full)]
    public class PhoebePlayerPawn : PlayerPawn
    {
    }

    [NetFieldExportGroup("/Game/Athena/AI/MANG/BP_MangPlayerPawn_Default.BP_MangPlayerPawn_Default_C", minimalParseMode: ParseMode.Full)]
    public class MangPlayerPawn : PlayerPawn
    {
    }

    [NetFieldExportGroup("/Game/Athena/AI/MANG/BP_MangPlayerPawn_Boss_AdventureGirl.BP_MangPlayerPawn_Boss_AdventureGirl_C", minimalParseMode: ParseMode.Full)]
    public class MangBossPlayerPawn : PlayerPawn
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
}

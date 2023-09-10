using FortniteReplayReader.Models.NetFieldExports.Vehicles;
using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Builds;

[NetFieldExportGroup("/Game/Building/ActorBlueprints/Player/Wood/L1/PBWA_W1_Floor.PBWA_W1_Floor_C", minimalParseMode: ParseMode.Debug)]
public class WoodFloor : BaseBuild
{
}

[NetFieldExportGroup("/Game/Building/ActorBlueprints/Player/Wood/L1/PBWA_W1_BalconyI.PBWA_W1_BalconyI_C", minimalParseMode: ParseMode.Debug)]
public class WoodBalconyIFloor : BaseBuild
{

}

[NetFieldExportGroup("/Game/Building/ActorBlueprints/Player/Wood/L1/PBWA_W1_BalconyS.PBWA_W1_BalconyS_C", minimalParseMode: ParseMode.Debug)]
public class WoodBalconySFloor : BaseBuild
{
}

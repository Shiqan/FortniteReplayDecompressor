using FortniteReplayReader.Models.NetFieldExports.Vehicles;
using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Builds;

/// <summary>
/// Every piece a player built (walls, floors, stairs, roofs and their edits) in wood, stone or metal.
/// The material and piece type are in the actor's class path, for example <c>PBWA_S1_Solid_C</c> (stone wall).
/// </summary>
[NetFieldExportGroupPrefix("/Game/Building/ActorBlueprints/Player/", minimalParseMode: ParseMode.Full)]
public class PlayerBuild : BaseBuild
{
}

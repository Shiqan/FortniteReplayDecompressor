using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_BuildingTools/Blueprints/DefaultBuildingTool.DefaultBuildingTool_C", minimalParseMode: ParseMode.Debug)]
public class BuildingTool : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_BuildingTools/Blueprints/DefaultEditingTool.DefaultEditingTool_C", minimalParseMode: ParseMode.Debug)]
public class EditingTool : BaseWeapon
{
}
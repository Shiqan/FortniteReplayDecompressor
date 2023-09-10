using Unreal.Core.Extensions;
using Xunit;

namespace Unreal.Core.Test;

public class StringExtensionsTest
{
    [Fact]
    public void CleanPathSuffixTest()
    {
        Assert.Equal("LF_7x12_Parent", "LF_7x12_Parent2_2".CleanPathSuffix());
        Assert.Equal("LF_Athena_POI_25x", "LF_Athena_POI_25x31_5".CleanPathSuffix());
        Assert.Equal("Apollo_Tree_RedAlder", "Apollo_Tree_RedAlder248".CleanPathSuffix());
        Assert.Equal("Apllo_Tree_Birch_Large", "Apllo_Tree_Birch_Large175".CleanPathSuffix());
        Assert.Equal("FortTeamPrivateInfo_ClassNetCache", "FortTeamPrivateInfo_ClassNetCache".CleanPathSuffix());
    }

    [Fact]
    public void RemovePathPrefixTest()
    {
        Assert.Equal("FortPickupAthena", "Default__FortPickupAthena".RemovePathPrefix("Default__"));
        Assert.Equal("AthenaAircraft_C", "Default__AthenaAircraft_C".RemovePathPrefix("Default__"));
        Assert.Equal("FortTeamPrivateInfo_ClassNetCache", "FortTeamPrivateInfo_ClassNetCache".RemovePathPrefix("Default__"));
        Assert.Equal("DamageSet", "DamageSet".RemovePathPrefix("Default__"));

        Assert.Equal("_ClassNetCache", "FortTeamPrivateInfo_ClassNetCache".RemovePathPrefix("FortTeamPrivateInfo"));
        Assert.Equal("Set", "DamageSet".RemovePathPrefix("Damage"));
        Assert.Equal("DamageSet", "DamageSet".RemovePathPrefix(""));
    }

    [Fact]
    public void RemoveAllPathPrefixesTest()
    {
        Assert.Equal("Prop_RockPile_06_C", "/Game/Athena/Apollo/Environments/BuildingActors/Rocks/Prop_RockPile_06.Prop_RockPile_06_C".RemoveAllPathPrefixes());
        Assert.Equal("PBWA_W1_StairW_C", "/Game/Building/ActorBlueprints/Player/Wood/L1/PBWA_W1_StairW.PBWA_W1_StairW_C".RemoveAllPathPrefixes());
        Assert.Equal("B_Shotgun_Standard_Athena_C", "/Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_Standard_Athena.B_Shotgun_Standard_Athena_C".RemoveAllPathPrefixes());
        Assert.Equal("/Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_Standard_Athena/B_Shotgun_Standard_Athena_C", "/Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_Standard_Athena/B_Shotgun_Standard_Athena_C".RemoveAllPathPrefixes());
    }
}

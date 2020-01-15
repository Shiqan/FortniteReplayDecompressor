using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Items
{
    public abstract class BaseContainer : INetFieldExportGroup
    {
        [NetFieldExport("bHidden", RepLayoutCmdType.Ignore)]
        public bool bHidden { get; set; }

        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
        public int RemoteRole { get; set; }

        [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
        public int Role { get; set; }

        [NetFieldExport("ForceMetadataRelevant", RepLayoutCmdType.Property)]
        public DebuggingObject ForceMetadataRelevant { get; set; }

        [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool)]
        public bool bDestroyed { get; set; }

        [NetFieldExport("StaticMesh", RepLayoutCmdType.PropertyObject)]
        public uint StaticMesh { get; set; }

        [NetFieldExport("AltMeshIdx", RepLayoutCmdType.PropertyUInt32)]
        public uint AltMeshIdx { get; set; }

        [NetFieldExport("WeaponData", RepLayoutCmdType.Property)]
        public ItemDefinition WeaponData { get; set; }

        [NetFieldExport("BuildTime", RepLayoutCmdType.Property)]
        public DebuggingObject BuildTime { get; set; }

        [NetFieldExport("RepairTime", RepLayoutCmdType.Property)]
        public DebuggingObject RepairTime { get; set; }

        [NetFieldExport("Health", RepLayoutCmdType.Property)]
        public DebuggingObject Health { get; set; }

        [NetFieldExport("MaxHealth", RepLayoutCmdType.Property)]
        public DebuggingObject MaxHealth { get; set; }

        [NetFieldExport("SearchedMesh", RepLayoutCmdType.Ignore)]
        public uint SearchedMesh { get; set; }

        [NetFieldExport("ReplicatedLootTier", RepLayoutCmdType.Property)]
        public DebuggingObject ReplicatedLootTier { get; set; }

        [NetFieldExport("bAlreadySearched", RepLayoutCmdType.PropertyBool)]
        public bool bAlreadySearched { get; set; }

        [NetFieldExport("BounceNormal", RepLayoutCmdType.Property)]
        public DebuggingObject BounceNormal { get; set; }

        [NetFieldExport("SearchAnimationCount", RepLayoutCmdType.Property)]
        public DebuggingObject SearchAnimationCount { get; set; }

        [NetFieldExport("ChosenRandomUpgrade", RepLayoutCmdType.PropertyInt)]
        public int? ChosenRandomUpgrade { get; set; }

        [NetFieldExport("bMirrored", RepLayoutCmdType.PropertyBool)]
        public bool bMirrored { get; set; }

        [NetFieldExport("ReplicatedDrawScale3D", RepLayoutCmdType.PropertyVector100)]
        public FVector ReplicatedDrawScale3D { get; set; }

        [NetFieldExport("bIsInitiallyBuilding", RepLayoutCmdType.PropertyBool)]
        public bool? bIsInitiallyBuilding { get; set; }

        [NetFieldExport("bForceReplayRollback", RepLayoutCmdType.PropertyBool)]
        public bool? bForceReplayRollback { get; set; }
    }
}
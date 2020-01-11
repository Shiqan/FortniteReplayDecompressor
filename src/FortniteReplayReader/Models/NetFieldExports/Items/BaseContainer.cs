using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    public abstract class BaseContainer
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

        [NetFieldExport("StaticMesh", RepLayoutCmdType.Property)]
        public DebuggingObject StaticMesh { get; set; }

        [NetFieldExport("AltMeshIdx", RepLayoutCmdType.Property)]
        public DebuggingObject WeaponData { get; set; }

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
    }
}
using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    public abstract class BaseProjectile
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
        public int RemoteRole { get; set; }

        [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
        public int Role { get; set; }

        [NetFieldExport("Owner", RepLayoutCmdType.Ignore)]
        public uint Owner { get; set; }

        [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
        public uint Instigator { get; set; }

        [NetFieldExport("Team", RepLayoutCmdType.PropertyByte)]
        public byte Team { get; set; }

        [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement)]
        [RepMovement(
            locationQuantizationLevel: VectorQuantization.RoundWholeNumber,
            rotationQuantizationLevel: RotatorQuantization.ByteComponents,
            velocityQuantizationLevel: VectorQuantization.RoundWholeNumber)]
        public FRepMovement ReplicatedMovement { get; set; }

        [NetFieldExport("ReplicatedMaxSpeed", RepLayoutCmdType.Property)]
        public DebuggingObject ReplicatedMaxSpeed { get; set; }

        [NetFieldExport("GravityScale", RepLayoutCmdType.PropertyFloat)]
        public float GravityScale { get; set; }
    }
}
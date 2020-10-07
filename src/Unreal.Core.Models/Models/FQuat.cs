using Unreal.Core.Models.Contracts;

namespace Unreal.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class FQuat : IProperty
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float W { get; private set; }

        //bool FQuat::NetSerialize(FArchive& Ar, class UPackageMap*, bool& bOutSuccess)
        public void Serialize(INetBitReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();

            var XYZMagSquared = (X * X + Y * Y + Z * Z);
            var WSquared = 1.0f - XYZMagSquared;
        }
    }
}
using Unreal.Core.Contracts;

namespace Unreal.Core.Models
{
    public class FHitResult : IProperty
    {
        public bool BlockingHit { get; private set; }
        public bool StartPenetrating { get; private set; }
        public int FaceIndex { get; private set; }
        public float Time { get; private set; }
        public float Distance { get; private set; }
        public FVector Location { get; private set; } //FVector_NetQuantize
        public FVector ImpactPoint { get; private set; } //FVector_NetQuantize
        public FVector Normal { get; private set; } //FVector_NetQuantizeNormal
        public FVector ImpactNormal { get; private set; } //FVector_NetQuantizeNormal
        public FVector TraceStart { get; private set; } //FVector_NetQuantize
        public FVector TraceEnd { get; private set; } //FVector_NetQuantize
        public float PenetrationDepth { get; private set; }
        public int Item { get; private set; }
        public uint PhysMaterial { get; private set; }
        public uint Actor { get; private set; }
        public uint Component { get; private set; }
        public string BoneName { get; private set; }
        public string MyBoneName { get; private set; }

        public void Serialize(NetBitReader reader)
        {
            var flags = reader.ReadBits(7);

            BlockingHit = flags[6];
            StartPenetrating = flags[5];
            var impactPointEqualsLocation = flags[4];
            var impactNormalEqualsNormal = flags[3];
            var invalidItem = flags[2];
            var invalidFaceIndex = flags[1];
            var noPenetrationDepth = flags[0];

            Time = reader.ReadSingle();
            Location = reader.SerializePropertyQuantizedVector();
            Normal = reader.SerializePropertyVectorNormal();

            if (!impactPointEqualsLocation)
            {
                ImpactPoint = reader.SerializePropertyQuantizedVector();
            }
            else
            {
                ImpactPoint = Location;
            }

            if (!impactNormalEqualsNormal)
            {
                ImpactNormal = reader.SerializePropertyVectorNormal();
            }
            else
            {
                ImpactNormal = Normal;
            }

            TraceStart = reader.SerializePropertyQuantizedVector();
            TraceEnd = reader.SerializePropertyQuantizedVector();

            if (!noPenetrationDepth)
            {
                PenetrationDepth = reader.SerializePropertyFloat();
            }
            else
            {
                PenetrationDepth = 0;
            }

            Distance = (ImpactPoint - TraceStart).Size();

            if (!invalidItem)
            {
                Item = reader.ReadBitsToInt(32);
            }
            else
            {
                Item = -1;
            }

            PhysMaterial = reader.SerializePropertyUInt16();
            Actor = reader.SerializePropertyUInt16();
            Component = reader.SerializePropertyUInt16();
            BoneName = reader.ReadFString();

            if (!invalidFaceIndex)
            {

                FaceIndex = reader.ReadBitsToInt(32);
            }
            else
            {
                FaceIndex = -1;
            }
        }
    }
}
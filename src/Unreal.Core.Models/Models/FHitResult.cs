using Unreal.Core.Models.Contracts;

namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/298e9af268d1982835fc86ca3e6cdb7fb2777a51/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L1926
    /// </summary>
    public class FHitResult : IProperty
    {
        public FHitResult()
        {

        }

        public FHitResult(INetBitReader reader)
        {
            Serialize(reader);
        }

        /// <summary>
        /// Indicates if this hit was a result of blocking collision. If false, there was no hit or it was an overlap/touch instead.
        /// </summary>
        public bool BlockingHit { get; private set; }

        /// <summary>
        /// Whether the trace started in penetration, i.e. with an initial blocking overlap.
        /// </summary>
        public bool StartPenetrating { get; private set; }

        /// <summary>
        /// Face index we hit (for complex hits with triangle meshes).
        /// </summary>
        public int FaceIndex { get; private set; }

        /// <summary>
        /// 'Time' of impact along trace direction (ranging from 0.0 to 1.0) if there is a hit, indicating time between TraceStart and TraceEnd.
        /// For swept movement(but not queries) this may be pulled back slightly from the actual time of impact, to prevent precision problems with adjacent geometry.
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// The distance from the TraceStart to the Location in world space. This value is 0 if there was an initial overlap (trace started inside another colliding object).
        /// </summary>
        public float Distance { get; private set; }

        /// <summary>
        /// The location in world space where the moving shape would end up against the impacted object, if there is a hit. Equal to the point of impact for line tests.
        /// </summary>
        public FVector Location { get; private set; } //FVector_NetQuantize

        /// <summary>
        /// Location in world space of the actual contact of the trace shape (box, sphere, ray, etc) with the impacted object.
        /// </summary>
        public FVector ImpactPoint { get; private set; } //FVector_NetQuantize

        /// <summary>
        /// Normal of the hit in world space, for the object that was swept. Equal to ImpactNormal for line tests.
        /// </summary>
        public FVector Normal { get; private set; } //FVector_NetQuantizeNormal

        /// <summary>
        /// Normal of the hit in world space, for the object that was hit by the sweep, if any.
        /// </summary>
        public FVector ImpactNormal { get; private set; } //FVector_NetQuantizeNormal

        /// <summary>
        /// Start location of the trace.
        /// </summary>
        public FVector TraceStart { get; private set; } //FVector_NetQuantize

        /// <summary>
        /// End location of the trace; this is NOT where the impact occurred (if any), but the furthest point in the attempted sweep.
        /// </summary>
        public FVector TraceEnd { get; private set; } //FVector_NetQuantize

        public float PenetrationDepth { get; private set; }

        /// <summary>
        /// Extra data about item that was hit (hit primitive specific).
        /// </summary>
        public int Item { get; private set; }

        /// <summary>
        /// Physical material that was hit.
        /// </summary>
        public uint PhysMaterial { get; private set; }

        /// <summary>
        /// Actor hit by the trace.
        /// </summary>
        public uint Actor { get; private set; }

        /// <summary>
        /// PrimitiveComponent hit by the trace.
        /// </summary>
        public uint Component { get; private set; }

        /// <summary>
        /// Name of bone we hit (for skeletal meshes).
        /// </summary>
        public string BoneName { get; private set; }

        /// <summary>
        /// Name of the _my_ bone which took part in hit event (in case of two skeletal meshes colliding).
        /// </summary>
        public string MyBoneName { get; private set; }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/c10022aa46e208b1593dd537c2607784aac158f1/Engine/Source/Runtime/Engine/Private/Collision/Collision.cpp#L42
        /// </summary>
        /// <param name="reader"></param>
        public void Serialize(INetBitReader reader)
        {
            // pack bitfield with flags
            //var flags = reader.ReadBits(7)[0];

            // Most of the time the vectors are the same values, use that as an optimization
            BlockingHit = reader.ReadBit();
            StartPenetrating = reader.ReadBit();
            bool bImpactPointEqualsLocation = reader.ReadBit();
            bool bImpactNormalEqualsNormal = reader.ReadBit();

            // Often times the indexes are invalid, use that as an optimization
            bool bInvalidItem = reader.ReadBit();
            bool bInvalidFaceIndex = reader.ReadBit();
            bool bNoPenetrationDepth = reader.ReadBit();

            Time = reader.ReadSingle();

            Location = reader.ReadPackedVector(1, 20);
            Normal = reader.SerializePropertyVectorNormal();

            if (!bImpactPointEqualsLocation)
            {
                ImpactPoint = reader.ReadPackedVector(1, 20);
            }
            else
            {
                ImpactPoint = Location;
            }

            if (!bImpactNormalEqualsNormal)
            {
                ImpactNormal = reader.SerializePropertyVectorNormal();
            }
            else
            {
                ImpactNormal = Normal;
            }

            TraceStart = reader.ReadPackedVector(1, 20);
            TraceEnd = reader.ReadPackedVector(1, 20);

            if (!bNoPenetrationDepth)
            {
                PenetrationDepth = reader.SerializePropertyFloat();
            }
            else
            {
                PenetrationDepth = 0.0f;
            }

            Distance = (ImpactPoint - TraceStart).Size();

            if (!bInvalidItem)
            {
                Item = reader.ReadInt32();
            }
            else
            {
                Item = 0;
            }

            PhysMaterial = reader.SerializePropertyObject();
            Actor = reader.SerializePropertyObject();
            Component = reader.SerializePropertyObject();
            BoneName = reader.SerializePropertyName();
            if (!bInvalidFaceIndex)
            {
                FaceIndex = reader.ReadInt32();
            }
            else
            {
                FaceIndex = 0;
            }
        }
    }
}
namespace Unreal.Core.Models;

public class FTransform
{
    /// <summary>
    /// Rotation of this transformation, as a quaternion.
    /// </summary>
    public FQuat Rotation { get; set; }

    /// <summary>
    ///  Translation of this transformation, as a vector.
    /// </summary>
    public FVector Translation { get; set; }

    /// <summary>
    /// 3D scale (always applied in local space) as a vector.
    /// </summary>
    public FVector Scale3D { get; set; }
}
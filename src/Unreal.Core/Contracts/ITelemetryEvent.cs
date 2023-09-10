namespace Unreal.Core.Contracts;

/// <summary>
/// Interface for all netfield events received through <see cref="ReplayReader{T}.OnExportRead(uint, INetFieldExportGroup)"/>.
/// </summary>
public interface ITelemetryEvent
{
    public float? ReplicatedWorldTimeSeconds { get; set; }

    public double? ReplicatedWorldTimeSecondsDouble { get; set; }
}

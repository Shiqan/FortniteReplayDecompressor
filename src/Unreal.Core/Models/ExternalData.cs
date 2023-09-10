using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

public class ExternalData : IExternalData
{
    public uint NetGUID { get; init; }
    public FArchive Archive { get; init; }
    public int TimeSeconds { get; init; }
}

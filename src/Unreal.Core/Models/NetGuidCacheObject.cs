namespace Unreal.Core.Models;

/// <summary>
/// 
/// </summary>
public class NetGuidCacheObject
{
    //public UObject Obj { get; private set; } = new UObject();

    public NetworkGUID OuterGuid { get; set; }
    public string PathName { get; set; }
    public uint NetworkChecksum { get; set; }
    public double ReadOnlyTimestamp { get; set; }
    public byte Flags { get; set; }
    public bool IsBroken { get; set; }
    public bool IsPending { get; set; }

    public bool NoLoad => (Flags & (1 << 0)) > 0 ? true : false;
    public bool IgnoreWhenMissing => (Flags & (1 << 1)) > 0 ? true : false;

    public override string ToString() => PathName;
}
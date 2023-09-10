using FortniteReplayReader.Models.NetFieldExports.RPC;
using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportClassNetCache("FortBroadcastRemoteClientInfo_ClassNetCache", minimalParseMode: ParseMode.Ignore)]
public class FortBroadcastRemoteClientInfoCache
{
    [NetFieldExportRPC("ClientRemotePlayerAddMapMarker", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerAddMapMarker", isFunction: true)]
    public AddMapMarker AddMapMarker { get; set; }

    [NetFieldExportRPC("ClientRemotePlayerRemoveMapMarker", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerRemoveMapMarker", isFunction: true)]
    public RemoveMapMarker RemoveMapMarker { get; set; }

    [NetFieldExportRPC("ClientRemotePlayerDamagedResourceBuilding", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerDamagedResourceBuilding", isFunction: true)]
    public PlayerDamagedResourceBuilding PlayerDamagedResourceBuilding { get; set; }

    //[NetFieldExportRPCFunction("ClientRemotePlayerHitMarkers", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerHitMarkers")]
    //public object PlayerHitMarkers { get; set; }
}

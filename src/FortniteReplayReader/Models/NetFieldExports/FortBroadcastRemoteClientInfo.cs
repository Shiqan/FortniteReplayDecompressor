using Unreal.Core.Attributes;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportClassNetCache("FortBroadcastRemoteClientInfo_ClassNetCache")]
    public class FortBroadcastRemoteClientInfoCache
    {
        [NetFieldExportRPCFunction("ClientRemotePlayerAddMapMarker", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerAddMapMarker")]
        public AddMapMarker AddMapMarker { get; set; }

        [NetFieldExportRPCFunction("ClientRemotePlayerRemoveMapMarker", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerRemoveMapMarker")]
        public RemoveMapMarker RemoveMapMarker { get; set; }

        [NetFieldExportRPCFunction("ClientRemotePlayerDamagedResourceBuilding", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerDamagedResourceBuilding")]
        public PlayerDamagedResourceBuilding PlayerDamagedResourceBuilding { get; set; }

        //[NetFieldExportRPCFunction("ClientRemotePlayerHitMarkers", "/Script/FortniteGame.FortBroadcastRemoteClientInfo:/Script/FortniteGame.FortBroadcastRemoteClientInfo:ClientRemotePlayerHitMarkers")]
        //public object PlayerHitMarkers { get; set; }
    }
}

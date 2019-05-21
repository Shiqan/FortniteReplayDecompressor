namespace FortniteReplayReaderDecompressor.Core.Models.Enums
{
    /// <summary>
    /// see https://api.unrealengine.com/INT/API/Runtime/Engine/Net/
    /// </summary>
    public enum NMT_Types
    {
        Hello = 0,                  // initial client connection message
        Welcome = 1,                // server tells client they're ok'ed to load the server's level
        Upgrade = 2,                // server tells client their version is incompatible
        Challenge = 3,              // server sends client challenge string to verify integrity
        NetSpeed = 4,               // client sends requested transfer rate
        Login = 5,                  // client requests to be admitted to the game
        Failure = 6,                // indicates connection failure
        Join = 9,                   // final join request (spawns PlayerController)
        JoinSplit = 10,             // child player (splitscreen) join request
        Skip = 12,                  // client request to skip an optional package
        Abort = 13,                 // client informs server that it aborted a not-yet-verified package due to an UNLOAD request
        PCSwap = 15,                // client tells server it has completed a swap of its Connection->Actor
        ActorChannelFailure = 16,   // client tells server that it failed to open an Actor channel sent by the server (e.g. couldn't serialize Actor archetype)
        DebugText = 17,             // debug text sent to all clients or to server
        NetGUIDAssign = 18,         // Explicit NetworkGUID assignment. This is rare and only happens if a netguid is only serialized client->server (this msg goes server->client to tell client what ID to use in that case)
        SecurityViolation = 19,     // server tells client that it has violated security and has been disconnected
        GameSpecific = 20,          // custom game-specific message routed to UGameInstance for processing
        EncryptionAck = 21,
        BeaconWelcome = 25,         // server tells client they're ok to attempt to join (client sends netspeed/beacontype)
        BeaconJoin = 26,            // server tries to create beacon type requested by client, sends NetGUID for actor sync
        BeaconAssignGUID = 27,      // client assigns NetGUID from server to beacon actor, sends NetGUIDAck
        BeaconNetGUIDAck = 28,      // server received NetGUIDAck from client, connection established successfully
    }
}
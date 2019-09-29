namespace Unreal.Core.Models
{
    /// <summary>
    /// A channel for exchanging actor and its subobject's properties and RPCs.
    /// ActorChannel manages the creation and lifetime of a replicated actor. Actual replication of properties and RPCs
    /// actually happens in FObjectReplicator now (see DataReplication.h).
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Engine/Classes/Engine/ActorChannel.h
    /// </summary>
    public class UActorChannel : UChannel
    {
        //public AActor Actor { get; set; }
        public NetworkGUID ActorNetGUID { get; set; }

        // TODO
    }
}

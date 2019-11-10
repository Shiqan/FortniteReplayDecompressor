namespace Unreal.Core.Models
{
    public class AthenaPlayerState : ActorState
    {
        public int PlayerId { get; set; }
        public int StartTime { get; set; }
        public int WorldPlayerId { get; set; }
        public string PlatformId { get; set; }
        public string UniqueId { get; set; }
        public string ColorId { get; set; }
        public string IconId { get; set; }
        public string PlayerNameId { get; set; }
        public string SkinName { get; set; }
        public bool StreamerMode { get; set; }
        public string PlayerNamePrivate { get; set; }
        public string PartyOwnerUniqueId { get; set; }
        public string Platform { get; set; }
        public int Team { get; set; }
        public int TeamIndex { get; set; }
        public int PlayerTeamPrivate { get; set; }
        public int SquadListUpdateValue { get; set; }
        public int SquadId { get; set; }
        public uint Level { get; set; }
        public bool bInAircraft { get; set; }
        public bool bHasFinishedLoading { get; set; }
        public bool bHasStartedPlaying { get; set; }
        public uint HeroType { get; set; }
        public int CharacterGender { get; set; }
        public int CharacterBodyType { get; set; }
        public uint Parts { get; set; }
        public int WasReplicatedFlags { get; set; }
        public FVector2D MapIndicatorPos { get; set; }
        public bool bUsingStreamerMode { get; set; }
        public bool bThankedBusDriver { get; set; }
        public bool bOnlySpectator { get; set; }
        public uint Ping { get; set; }
        public uint Owner { get; set; }
        public bool bIsDisconnected { get; set; }
        public uint FinisherOrDowner { get; set; }
        public bool bInitialized { get; set; }
        public bool bUsingAnonymousMode { get; set; }
        public float ResurrectionExpirationTime { get; set; }
        public uint DeathTags { get; set; } // FGameplayTagContainer
    }
}

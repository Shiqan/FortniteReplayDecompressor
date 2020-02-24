using FortniteReplayReader.Models.NetFieldExports;
using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models
{
    public class PlayerData
    {
        public PlayerData(FortPlayerState playerState)
        {
            Id = playerState.PlayerID;
            EpicId = playerState.UniqueId;
            BotId = playerState.BotUniqueId;
            IsBot = playerState.bIsABot;
            PlayerNameCustomOverride = playerState.PlayerNameCustomOverride?.Text;
            IsGameSessionOwner = playerState.bIsGameSessionOwner;
            PlayerName = playerState.PlayerNamePrivate;
            StreamerModeName = playerState.StreamerModeName?.Text;
            IsPartyLeader = playerState.PartyOwnerUniqueId == playerState.UniqueId;
            TeamIndex = playerState.TeamIndex;
            Level = playerState.Level;
            SeasonLevelUIDisplay = playerState.SeasonLevelUIDisplay;
            Platform = playerState.Platform;
            HasFinishedLoading = playerState.bHasFinishedLoading;
            HasStartedPlaying = playerState.bHasStartedPlaying;
            IsUsingAnonymousMode = playerState.bUsingAnonymousMode;
            IsUsingStreamerMode = playerState.bUsingStreamerMode;
            HasEverSkydivedFromBus = playerState.bHasEverSkydivedFromBus;
            HasEverSkydivedFromBusAndLanded = playerState.bHasEverSkydivedFromBusAndLanded;

            Cosmetics = new Cosmetics()
            {
                CharacterBodyType = playerState.CharacterBodyType,
                HeroType = playerState.HeroType?.Name,
                CharacterGender = playerState.CharacterGender
            };
        }

        public int? Id { get; set; }
        public string PlayerId => (IsBot == true) ? BotId : EpicId;
        public string EpicId { get; set; }
        public string BotId { get; set; }
        public bool? IsBot { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNameCustomOverride { get; set; }
        public string StreamerModeName { get; set; }
        public string Platform { get; set; }
        public int? Level { get; set; }
        public uint? SeasonLevelUIDisplay { get; set; }

        public uint? InventoryId { get; set; }

        public int? PlayerNumber { get; set; }
        public int? TeamIndex { get; set; }
        public bool IsPartyLeader { get; set; }
        public bool IsReplayOwner { get; set; }
        public bool? IsGameSessionOwner { get; set; }
        public bool? HasFinishedLoading { get; set; }
        public bool? HasStartedPlaying { get; set; }
        public bool? HasThankedBusDriver { get; set; }
        public bool? IsUsingStreamerMode { get; set; }
        public bool? IsUsingAnonymousMode { get; set; }

        public uint? RebootCounter { get; set; }
        public int? Placement { get; set; }
        public uint? Kills { get; set; }
        public uint? TeamKills { get; set; }
        public int? DeathCause { get; set; }
        public int? DeathCircumstance { get; set; }
        public IEnumerable<string> DeathTags { get; set; }
        public FVector DeathLocation { get; set; }

        public bool? HasEverSkydivedFromBus { get; set; }
        public bool? HasEverSkydivedFromBusAndLanded { get; set; }

        public Cosmetics Cosmetics { get; set; }
    }

    public class Cosmetics
    {
        public int? CharacterGender { get; set; }
        public int? CharacterBodyType { get; set; }
        public string Parts { get; set; }
        public IEnumerable<string> VariantRequiredCharacterParts { get; set; }
        public string HeroType { get; set; }
        public string BannerIconId { get; set; }
        public string BannerColorId { get; set; }
        public IEnumerable<string> ItemWraps { get; set; }
        public string SkyDiveContrail { get; set; }
        public string Glider { get; set; }
        public string Pickaxe { get; set; }
        public bool? IsDefaultCharacter { get; set; }
        public string Character { get; set; }
        public string Backpack { get; set; }
        public string LoadingScreen { get; set; }
        public IEnumerable<string> Dances { get; set; }
        public string MusicPack { get; set; }
        public string PetSkin { get; set; }
    }
}

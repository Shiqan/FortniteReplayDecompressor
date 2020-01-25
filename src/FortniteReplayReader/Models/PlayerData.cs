using System.Collections.Generic;

namespace FortniteReplayReader.Models
{
    public class PlayerData
    {
        public string PlayerId => IsBot ? BotId : EpicId;
        public string EpicId { get; set; }
        public string BotId { get; set; }
        public bool IsBot { get; set; }

        public string StreamerModeName { get; set; }
        public int Level { get; set; }

        public string Platform { get; set; }
        public bool IsGameSessionOwner { get; set; }
        public bool HasFinishedLoading { get; set; }
        public bool HasStartedPlaying { get; set; }
        public bool HasThankedBusDriver { get; set; }
        public bool IsUsingStreamerMode { get; set; }
        public bool IsUsingAnonymousMode { get; set; }

        public uint RebootCounter { get; set; }

        public bool HasEverSkydivedFromBus { get; set; }
        public bool HasEverSkydivedFromBusAndLanded { get; set; }

        public Cosmetics Cosmetics { get; set; }
    }

    public class Cosmetics
    {
        public int CharacterGender { get; set; }
        public int CharacterBodyType { get; set; }
        public string Parts { get; set; }
        public IEnumerable<string> VariantRequiredCharacterParts { get; set; }
        public string HeroType { get; set; }
        public string BannerIconId { get; set; }
        public string BannerColorId { get; set; }
        public IEnumerable<string> ItemWraps { get; set; }
        public string SkyDiveContrail { get; set; }
        public string Glider { get; set; }
        public string Pickaxe { get; set; }
        public bool IsDefaultCharacter { get; set; }
        public string Character { get; set; }
        public string Backpack { get; set; }
        public string LoadingScreen { get; set; }
        public IEnumerable<string> Dances { get; set; }
        public string MusicPack { get; set; }
        public string PetSkin { get; set; }
    }
}

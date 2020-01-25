namespace FortniteReplayReader.Models
{
    public class KillFeedEntry
    {
        public string PlayerId { get; set; }
        public string FinisherOrDowner { get; set; }
        public int ItemId { get; set; }
        public float TimeSeconds { get; set; }
        public float Distance { get; set; }
        public string Weapon { get; set; }
        public bool IsDowned { get; set; }
        public bool IsRevived { get; set; } // split in another class?
    }
}

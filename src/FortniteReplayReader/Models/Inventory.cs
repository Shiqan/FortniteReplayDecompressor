using System.Collections.Generic;

namespace FortniteReplayReader.Models;

public class Inventory
{
    public uint? Id { get; set; }
    public uint? ReplayPawn { get; set; }
    public int? PlayerId { get; set; }
    public string PlayerName { get; set; }
    public IList<InventoryItem> Items { get; set; } = new List<InventoryItem>();
}

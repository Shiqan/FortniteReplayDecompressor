namespace FortniteReplayReader.Models;

public class InventoryItem
{
    public int? Count { get; set; }
    public string ItemDefinition { get; set; }
    public ushort? OrderIndex { get; set; }
    public float? Durability { get; set; }
    public int? Level { get; set; }
    public int? LoadedAmmo { get; set; }
    public uint? A { get; set; }
    public uint? B { get; set; }
    public uint? C { get; set; }
    public uint? D { get; set; }
}

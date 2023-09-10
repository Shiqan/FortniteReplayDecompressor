namespace FortniteReplayReader.Models;

public class WeaponData
{
    public bool? bIsEquippingWeapon { get; set; }
    public bool? bIsReloadingWeapon { get; set; }
    public string WeaponName { get; set; }
    public float? LastFireTimeVerified { get; set; }
    public int? WeaponLevel { get; set; }
    public int? AmmoCount { get; set; }

    public uint? A { get; set; }
    public uint? B { get; set; }
    public uint? C { get; set; }
    public uint? D { get; set; }
}

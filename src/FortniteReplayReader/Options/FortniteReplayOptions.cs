namespace FortniteReplayReader.Options;

public enum LocationTypes
{
    /// <summary>
    /// Grab all possible locations from any player.
    /// </summary>
    All,
    /// <summary>
    /// Grab locations from replay owner and all teammates.
    /// </summary>
    Team,
    /// <summary>
    /// Only grab locations from replay owner.
    /// </summary>
    User,
    /// <summary>
    /// Grab no locations.
    /// </summary>
    None
};

public class FortniteReplayOptions
{
    /// <summary>
    /// Which players to grab locations for. 
    /// </summary>
    public LocationTypes PlayerLocationType { get; set; } = LocationTypes.All;

    /// <summary>
    /// Total time, in milliseconds, required for a location change to be saved.
    /// 0 = Grab all.
    /// </summary>
    public int LocationChangeDeltaMS { get; set; } = 0;

    /// <summary>
    /// Ignores health updates.
    /// </summary>
    public bool IgnoreHealth { get; set; }

    /// <summary>
    /// Ignores containers
    /// </summary>
    public bool IgnoreContainers { get; set; }

    /// <summary>
    /// Ignores shots.
    /// </summary>
    public bool IgnoreShots { get; set; }

    /// <summary>
    /// Ignores inventory changes.
    /// </summary>
    public bool IgnoreInventory { get; set; }

    /// <summary>
    /// Ignores floor loot.
    /// </summary>
    public bool IgnoreFloorLoot { get; set; }

    /// <summary>
    /// Ignores storing the weapon switches for player.
    /// </summary>
    public bool IgnoreWeaponSwitches { get; set; }
}

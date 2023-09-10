using System;

namespace FortniteReplayReader.Models.Events;

public class PlayerElimination : BaseEvent, IEquatable<PlayerElimination>
{
    public PlayerEliminationInfo EliminatedInfo { get; internal set; } = new PlayerEliminationInfo();
    public PlayerEliminationInfo EliminatorInfo { get; internal set; } = new PlayerEliminationInfo();

    public string? Eliminated => EliminatedInfo?.Id;
    public string? Eliminator => EliminatorInfo?.Id;
    public byte GunType { get; internal set; }
    public string Time { get; internal set; }
    public bool Knocked { get; internal set; }
    public bool IsSelfElimination => Eliminated == Eliminator;
    public bool IsValidLocation => EliminatorInfo.Location.Size() != 0;
    public double? Distance => IsValidLocation ? EliminatorInfo.Location.DistanceTo(EliminatedInfo.Location) : null;

    public bool Equals(PlayerElimination other)
    {
        if (other is null)
        {
            return false;
        }

        if (Eliminated == other.Eliminated && Eliminator == other.Eliminator && GunType == other.GunType && Time == other.Time && Knocked == other.Knocked)
        {
            return true;
        }

        return false;
    }
}
namespace Unreal.Core.Models;

/// <summary>
/// Actor is the base class for an Object that can be placed or spawned in a level.
/// Actors may contain a collection of ActorComponents, which can be used to control how actors move, how they are rendered, etc.
/// The other main function of an Actor is the replication of properties and function calls across the network during play.
/// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Engine/Classes/GameFramework/Actor.h
/// </summary>
public class Actor
{
    public NetworkGUID ActorNetGUID { get; set; }
    public NetworkGUID? Archetype { get; set; }
    public NetworkGUID? Level { get; set; }
    public FVector? Location { get; set; }
    public FRotator? Rotation { get; set; }
    public FVector? Scale { get; set; }
    public FVector? Velocity { get; set; }
}

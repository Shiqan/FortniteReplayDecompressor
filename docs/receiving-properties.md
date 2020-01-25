We took the liberty to implement most (if not all) Fortnite groups for you. 
However, if you want to know the details or leverage the Unreal.Core reader for another game, here is how it works.

### Setup

In order to receive properties, you have to mark classes with the `NetFieldExportGroup` attribute. This property requires a path argument in order to map the NetFieldExportGroup to your class. 

``` csharp
    [NetFieldExportGroup("/Game/Athena/PlayerPawn_Athena.PlayerPawn_Athena_C")]
    public class PlayerPawn : INetFieldExportGroup
```
Additionally you can also pass the `minimalParseMode`, which will only parse that class if the parse mode is equal or higher than the parse mode you marked your class with.

The properties themselves should be mapped with a `NetFieldExport` attribute, which is a mapping based on the name (or in case of the `NetFieldExportHandleAttribute` the handle). This attribute requires a second argument, the `RepLayoutCmdType`, which tells us how to parse this particular property.

``` csharp
	[NetFieldExport("Role", RepLayoutCmdType.Ignore)]
	public object Role { get; set; }
```

In some cases, the names are not included in the replay. So we'll have use their handles (after spending hours in order to figure out what they mean of course).

``` csharp
	[NetFieldExportHandle(1, RepLayoutCmdType.PropertyFloat)]
	public float HealthCurrentValue { get; set; }
```

### Receiving
Once configured, you can implement the abstract `OnExportRead(uint channel, INetFieldExportGroup exportGroup)` method which is called after a group is fully read while parsing.
It contains the current channel id and (obviously) the exportGroup.
If you have read the [Receiving Properties](https://fortnitereplaydecompressor.readthedocs.io/en/latest/receiving-properties/) page you would have already seen how we handle properties. RPC (or functions) are quite similar so you'll quickly get the hang of it.

### ClassNetCache

In Unreal Engine, the ClassNetCache contains information about a class, cached for network coordination. This could be structs, but more often than not (at least in Fortnite) it are functions that should be called in order to sync the state.

### Setup
So to get started with the ClassNetCache, mark your classes with the `NetFieldExportClassNetCache` attribute.

``` csharp
    [NetFieldExportClassNetCache("PlayerPawn_Athena_C_ClassNetCache")]
    public class PlayerPawnCache
```

The properties should be marked with the `NetFieldExportRPC` attribute. By default this indicates a struct, however, you can mark them as functions as well.

``` csharp
	[NetFieldExportRPC("ClientObservedStats", "/Script/FortniteGame.FortClientObservedStat")]
	public FortClientObservedStat ClientObservedStats { get; set; }
	
	NetFieldExportRPC("NetMulticast_Athena_BatchedDamageCues", "/Script/FortniteGame.FortPawn:NetMulticast_Athena_BatchedDamageCues", isFunction: true)]
	public BatchedDamageCues FastSharedReplication { get; set; }
```

### Receiving
Once again, after successfully parsing a rpc struct or function, the abstract method `OnExportRead(uint channel, INetFieldExportGroup exportGroup)` will be called.
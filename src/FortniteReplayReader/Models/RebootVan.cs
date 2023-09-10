using FortniteReplayReader.Models.NetFieldExports;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models;

public class RebootVan
{
    public RebootVan()
    {

    }

    public RebootVan(SpawnMachineRepData spawnMachine)
    {
        Id = spawnMachine.SpawnMachineRepDataHandle;
        Location = spawnMachine.Location;
        SpawnMachineState = spawnMachine.SpawnMachineState;
        SpawnMachineCooldownStartTime = spawnMachine.SpawnMachineCooldownStartTime;
        SpawnMachineCooldownEndTime = spawnMachine.SpawnMachineCooldownEndTime;
    }

    public int Id { get; set; }
    public FVector Location { get; set; }
    public int SpawnMachineState { get; set; }
    public float SpawnMachineCooldownStartTime { get; set; }
    public float SpawnMachineCooldownEndTime { get; set; }
}

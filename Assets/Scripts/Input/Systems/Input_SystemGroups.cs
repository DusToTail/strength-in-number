using Unity.Entities;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class Input_Initialization_SystemGroup : ComponentSystemGroup
    {
    }
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class Input_Simulation_SystemGroup : ComponentSystemGroup
    {
    }
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class Input_Reset_SystemGroup : ComponentSystemGroup
    {
    }
}

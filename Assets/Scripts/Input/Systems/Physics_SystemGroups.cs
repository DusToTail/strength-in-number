using Unity.Entities;


namespace StrengthInNumber
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(Input_Initialization_SystemGroup))]
    public partial class Physics_Initialization_SystemGroup : ComponentSystemGroup
    {
    }
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial class Physics_QueryAfter_SystemGroup : ComponentSystemGroup
    {
    }
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(Input_Reset_SystemGroup))]
    public partial class Physics_Reset_SystemGroup : ComponentSystemGroup
    {
    }
}

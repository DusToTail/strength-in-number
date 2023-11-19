using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

namespace StrengthInNumber.Input
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct ResetInputSystem : ISystem
    {
        private EntityQuery _query;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAllRW<Mouse, Keyboard>();
            _query = builder.Build(ref state);
            builder.Dispose();
            state.RequireForUpdate<Mouse>();
            state.RequireForUpdate<Keyboard>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _query.GetSingletonRW<Mouse>().ValueRW = default;
            _query.GetSingletonRW<Keyboard>().ValueRW = default;
        }
    }
}

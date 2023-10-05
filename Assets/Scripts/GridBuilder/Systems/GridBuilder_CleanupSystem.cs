using Unity.Entities;
using Unity.Burst;

using UnmanagedGrid = StrengthInNumber.GridBuilder.GridBuilder_UnmanagedGrid;

namespace StrengthInNumber.GridBuilder
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct GridBuilder_CleanupSystem : ISystem, ISystemStartStop
    {
        private EntityQuery _query;
        private Entity _entity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            GridBuilderUtils.GetQuery(state.WorldUnmanaged.EntityManager, out _query);
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _entity = _query.GetSingletonEntity();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            var unmanagedGrid = state.EntityManager.GetComponentData<UnmanagedGrid>(_entity);
            unmanagedGrid.Dispose(state.Dependency).Complete();
            state.WorldUnmanaged.EntityManager.DestroyEntity(_entity);
            _entity = Entity.Null;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _query.Dispose();
        }
    }
}

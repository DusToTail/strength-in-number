using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

namespace StrengthInNumber.GridBuilder
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct PlatformCleanupSystem : ISystem, ISystemStartStop
    {
        private EntityQuery _query;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlatformData>();
        }
        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            var em = state.WorldUnmanaged.EntityManager;
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlatformData>();
            _query = builder.Build(em);
            builder.Dispose();
        }
        public void OnStopRunning(ref SystemState state)
        {
            var em = state.WorldUnmanaged.EntityManager;

            var platforms = _query.ToEntityArray(Allocator.Temp);

            foreach (var platform in platforms)
            {
                var data = em.GetComponentData<PlatformData>(platform);
                data.positions.Dispose();
            }

            em.RemoveComponent(platforms, typeof(PlatformData));
            em.DestroyEntity(platforms);
            platforms.Dispose();
            _query.Dispose();
        }
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}

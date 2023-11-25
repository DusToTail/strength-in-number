using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Transforms;

namespace StrengthInNumber.Builder
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BuilderSystem))]
    public partial struct SpawnSystem : ISystem
    {
        private BufferLookup<BuildPrefab> _prefabsLookup;
        private DynamicBuffer<BuildPrefab> _prefabs;
        private EntityQuery _builderQ;
        private EntityQuery _buildQ;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<BuildPrefab>();
                _builderQ = builder.Build(ref state);
                builder.Dispose();
            }
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<BuildEntity>();
                _buildQ = builder.Build(ref state);
                builder.Dispose();
            }
            _prefabsLookup = state.GetBufferLookup<BuildPrefab>(true);
            state.RequireForUpdate(_builderQ);
            state.RequireForUpdate(_buildQ);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _prefabsLookup.Update(ref state);
            _prefabsLookup.TryGetBuffer(_builderQ.GetSingletonEntity(), out _prefabs);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            new SpawnEntityJob()
            {
                prefabs = _prefabs,
                ecb = ecb.AsParallelWriter()
            }.ScheduleParallel(_buildQ, state.Dependency).Complete();
            ecb.DestroyEntity(_buildQ, EntityQueryCaptureMode.AtPlayback);
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    partial struct SpawnEntityJob : IJobEntity
    {
        [ReadOnly] public DynamicBuffer<BuildPrefab> prefabs;
        [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int sortKey, in BuildEntity build)
        {
            var e = ecb.Instantiate(sortKey, prefabs[build.prefabIndex].prefab);
            ecb.SetComponent(sortKey, e, LocalTransform.FromPositionRotationScale(build.position, build.rotation, build.scale));
        }
    }
}

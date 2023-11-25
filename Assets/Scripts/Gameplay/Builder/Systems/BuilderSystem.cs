using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Transforms;

namespace StrengthInNumber.Builder
{
    public partial struct BuilderSystem : ISystem
    {
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
            state.GetBufferLookup<BuildPrefab>(true).TryGetBuffer(_builderQ.GetSingletonEntity(), out _prefabs);
            state.RequireForUpdate(_builderQ);
            state.RequireForUpdate(_buildQ);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            new BuildEntityJob()
            {
                prefabs = _prefabs,
                ecb = ecb.AsParallelWriter()
            }.ScheduleParallel(_buildQ, state.Dependency).Complete();
            ecb.DestroyEntity(_buildQ, EntityQueryCaptureMode.AtPlayback);
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    partial struct BuildEntityJob : IJobEntity
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

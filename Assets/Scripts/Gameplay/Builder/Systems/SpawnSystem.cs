using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using StrengthInNumber.Gameplay;
using StrengthInNumber.Grid;
using StrengthInNumber.Entities;

namespace StrengthInNumber.Builder
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct SpawnSystem : ISystem
    {
        private BufferLookup<BuildPrefab> _prefabsLookup;
        private DynamicBuffer<BuildPrefab> _prefabs;
        private EntityQuery _builderQ;
        private EntityQuery _buildQ;
        private EntityQuery _initializeQ;
        private EntityQuery _gridQ;

        private ComponentLookup<GridPosition> _gridPositionLookup;
        private ComponentLookup<Movement> _movementLookup;
        private ComponentLookup<LocalTransform> _transformLookup;

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
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<InitilizationTag>()
                    .WithOptions(EntityQueryOptions.IncludeDisabledEntities);
                _initializeQ = builder.Build(ref state);
                builder.Dispose();
            }
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAllRW<SquareGrid>();
                _gridQ = builder.Build(ref state);
                builder.Dispose();
            }
            _prefabsLookup = state.GetBufferLookup<BuildPrefab>(true);
            _movementLookup = state.GetComponentLookup<Movement>(true);
            _transformLookup = state.GetComponentLookup<LocalTransform>(true);
            _gridPositionLookup = state.GetComponentLookup<GridPosition>(true);
            state.RequireForUpdate(_builderQ);
            state.RequireForUpdate(_buildQ);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _prefabsLookup.Update(ref state);
            _prefabsLookup.TryGetBuffer(_builderQ.GetSingletonEntity(), out _prefabs);
            _gridPositionLookup.Update(ref state);
            var squareGrid = _gridQ.GetSingletonRW<SquareGrid>();

            {
                var ecb = new EntityCommandBuffer(Allocator.TempJob);
                new SpawnJob()
                {
                    prefabs = _prefabs,
                    squareGrid = (RefRO<SquareGrid>)squareGrid,
                    ecb = ecb.AsParallelWriter(),
                }.ScheduleParallel(_buildQ, state.Dependency).Complete();
                ecb.DestroyEntity(_buildQ, EntityQueryCaptureMode.AtPlayback);
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }

            _movementLookup.Update(ref state);
            _transformLookup.Update(ref state);

            {
                var ecb = new EntityCommandBuffer(Allocator.TempJob);
                new InitializeJob()
                {
                    movementLookup = _movementLookup,
                    transformLookup = _transformLookup,
                    ecb = ecb.AsParallelWriter()
                }.ScheduleParallel(_initializeQ, state.Dependency).Complete();
                ecb.RemoveComponent<InitilizationTag>(_initializeQ, EntityQueryCaptureMode.AtPlayback);
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }
            
        }
    }

    [BurstCompile]
    partial struct SpawnJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]
        public RefRO<SquareGrid> squareGrid;
        [ReadOnly] public DynamicBuffer<BuildPrefab> prefabs;
        [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int sortKey, in BuildEntity build)
        {
            var e = ecb.Instantiate(sortKey, prefabs[build.prefabIndex].prefab);
            ecb.SetEnabled(sortKey, e, false);

            ecb.SetComponent(sortKey, e, new GridPosition()
            {
                position = build.position
            });
            float3 position = default;
            quaternion rotation = quaternion.identity;

            if(build.gridType == Builder.GridType.Square)
            {
                position = squareGrid.ValueRO.GridToWorld(build.position.x, build.position.y);
                position.y += squareGrid.ValueRO.CellSize / 2f;

                if(build.faceEnum != 0)
                {
                    var face = (SquareGridUtils.Faces)build.faceEnum;
                    var dirInt2 = SquareGridUtils.ToInt2(face);
                    float3 dir = new float3(dirInt2.x, 0f, dirInt2.y);
                    rotation = quaternion.LookRotation(dir, new float3(0f, 1f, 0f));
                }
            }

            ecb.SetComponent(sortKey, e, LocalTransform.FromPositionRotation(position, rotation));
            ecb.AddComponent(sortKey, e, new InitilizationTag());
        }
    }

    [BurstCompile]
    partial struct InitializeJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> transformLookup;
        [ReadOnly] public ComponentLookup<Movement> movementLookup;
        [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int sortKey, Entity e)
        {
            var transform = transformLookup.GetRefRO(e).ValueRO;

            if (movementLookup.HasComponent(e))
            {
                var movement = movementLookup.GetRefRO(e).ValueRO;
                ecb.SetComponent(sortKey, e, new Movement()
                {
                    fromPosition = transform.Position,
                    toPosition = transform.Position,
                    fromRotation = transform.Rotation,
                    lerp = movement.lerp,
                    speed = movement.speed
                });
            }
            ecb.SetEnabled(sortKey, e, true);
        }
    }
}

using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using StrengthInNumber.Grid;
using StrengthInNumber.Entities;

namespace StrengthInNumber.Gameplay
{
    [UpdateBefore(typeof(MovingSystem))]
    public partial struct MovingValidationSystem : ISystem
    {
        private EntityQuery _movingCubeQ;
        private EntityQuery _idleCubeQ;
        private EntityQuery _squareGridQ;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAspect<MoveAspect>()
                    .WithAll<GridPosition, Cube, IsMoving>();
                _movingCubeQ = builder.Build(state.EntityManager);
                builder.Dispose();
            }

            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAspect<MoveAspect>()
                    .WithAll<GridPosition, Cube>()
                    .WithDisabled<IsMoving>();
                _idleCubeQ = builder.Build(state.EntityManager);
                builder.Dispose();
            }

            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<SquareGrid>();
                _squareGridQ = builder.Build(state.EntityManager);
                builder.Dispose();
            }
            var cubeQ = new NativeArray<EntityQuery>(2, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            cubeQ[0] = _movingCubeQ;
            cubeQ[1] = _idleCubeQ;
            state.RequireAnyForUpdate(cubeQ);
            state.RequireForUpdate(_squareGridQ);
            cubeQ.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _movingCubeQ.Dispose();
            _idleCubeQ.Dispose();
            _squareGridQ.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var grid = _squareGridQ.GetSingleton<SquareGrid>();
            var moving = _movingCubeQ.ToEntityArray(Allocator.Temp);
            var idle = _idleCubeQ.ToEntityArray(Allocator.Temp);

            {
                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
                new CheckMovingJob()
                {
                    ecb = ecb.AsParallelWriter(),
                    grid = grid
                }.ScheduleParallel(_movingCubeQ, state.Dependency).Complete();
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }

            {
                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
                new CheckIdleJob()
                {
                    ecb = ecb.AsParallelWriter(),
                    grid = grid
                }.ScheduleParallel(_idleCubeQ, state.Dependency).Complete();
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }

            moving.Dispose();
            idle.Dispose();
        }
    }

    [BurstCompile]
    partial struct CheckMovingJob : IJobEntity
    {
        [ReadOnly]
        public SquareGrid grid;
        public EntityCommandBuffer.ParallelWriter ecb;
        public void Execute(Entity e, [ChunkIndexInQuery] int sortKey, MoveAspect moveAspect, ref GridPosition position, ref Cube cube)
        {
            if(moveAspect.Lerp >= 1)
            {
                position.position += SquareGridUtils.ToInt2(cube.forward);
                cube.forward = SquareGridUtils.Faces.None;
                moveAspect.Reset();
                ecb.SetComponentEnabled<IsMoving>(sortKey, e, false);
            }
        }
    }

    [BurstCompile]
    partial struct CheckIdleJob : IJobEntity
    {
        [ReadOnly]
        public SquareGrid grid;
        public EntityCommandBuffer.ParallelWriter ecb;
        public void Execute(Entity e, [ChunkIndexInQuery] int sortKey, MoveAspect moveAspect, in GridPosition position, in Cube cube)
        {
            if(cube.forward != SquareGridUtils.Faces.None)
            {
                var toInt = position.position + SquareGridUtils.ToInt2(cube.forward);
                var toFloat = grid.GridToWorld(toInt.x, toInt.y);
                moveAspect.ToPosition = new Unity.Mathematics.float3(toFloat.x, moveAspect.FromPosition.y, toFloat.z);
                ecb.SetComponentEnabled<IsMoving>(sortKey, e, true);
            }
        }
    }
}

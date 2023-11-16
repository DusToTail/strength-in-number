using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using StrengthInNumber.Grid;
using StrengthInNumber.Entities;

namespace StrengthInNumber.Gameplay
{
    public partial struct MovingSystem : ISystem
    {
        private EntityQuery _cubeQ;
        private EntityQuery _squareGridQ;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAspect<MoveAspect>()
                    .WithAll<GridPosition, Cube, IsMoving>();
                _cubeQ = builder.Build(state.EntityManager);
                builder.Dispose();
            }

            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<SquareGrid>();
                _squareGridQ = builder.Build(state.EntityManager);
                builder.Dispose();
            }

            state.RequireForUpdate(_cubeQ);
            state.RequireForUpdate(_squareGridQ);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _cubeQ.Dispose();
            _squareGridQ.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var grid = _squareGridQ.GetSingleton<SquareGrid>();
            var entities = _cubeQ.ToEntityArray(Allocator.Temp);
            float dt = SystemAPI.Time.DeltaTime;

            new MoveEntitiesJob()
            {
                deltaTime = dt,
                rotateRadius = grid.CellSize / math.SQRT2,
                rotateAngle = math.radians(90f)

            }.ScheduleParallel(_cubeQ, state.Dependency).Complete();

            entities.Dispose();
        }
    }

    [BurstCompile]
    partial struct MoveEntitiesJob : IJobEntity
    {
        public float deltaTime;
        public float rotateRadius;
        public float rotateAngle;
        [ReadOnly]
        public SquareGrid grid;
        public void Execute(MoveAspect moveAspect)
        {
            moveAspect.Move(deltaTime, rotateRadius, rotateAngle);
        }
    }
}

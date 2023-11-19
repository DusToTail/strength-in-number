using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using StrengthInNumber.Entities;
using StrengthInNumber.Grid;
using StrengthInNumber.Input;

namespace StrengthInNumber.Gameplay
{
    [UpdateBefore(typeof(MovingValidationSystem))]
    public partial struct ControlSystem : ISystem
    {
        private EntityQuery _controllableQ;
        private EntityQuery _keyboardQ;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<Keyboard>();
                _keyboardQ = builder.Build(state.EntityManager);
                builder.Dispose();
            }

            {
                EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ControllableTag, Cube>()
                .WithDisabled<IsMoving>();
                _controllableQ = builder.Build(state.EntityManager);
                builder.Dispose();
            }
            state.RequireForUpdate(_controllableQ);
            state.RequireForUpdate(_keyboardQ);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _keyboardQ.Dispose();
            _controllableQ.Dispose();
        }

        [BurstCompile]
        partial struct ControlJob : IJobEntity
        {
            public bool up, down, left, right;
            public void Execute(ref Cube cube)
            {
                var face = SquareGridUtils.Faces.None;
                if (up)
                {
                    face = SquareGridUtils.Faces.Front;
                }
                else if(down)
                {
                    face = SquareGridUtils.Faces.Back;
                }
                else if(left)
                {
                    face = SquareGridUtils.Faces.Left;
                }
                else if(right)
                {
                    face = SquareGridUtils.Faces.Right;
                }
                // TODO: Validation when controlling
                cube.forward = face;
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // TODO: Refactor
            var keyboard = _keyboardQ.GetSingletonRW<Keyboard>();

            new ControlJob()
            {
                up = keyboard.ValueRO.up,
                down = keyboard.ValueRO.down,
                left = keyboard.ValueRO.left,
                right = keyboard.ValueRO.right,
            }.ScheduleParallel(_controllableQ, state.Dependency).Complete();

            keyboard.ValueRW.up = false;
            keyboard.ValueRW.down = false;
            keyboard.ValueRW.left = false;
            keyboard.ValueRW.right = false;
        }
    }
}

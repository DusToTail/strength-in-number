using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using StrengthInNumber.Entities;
using StrengthInNumber.Grid;

namespace StrengthInNumber.Gameplay
{
    [UpdateBefore(typeof(MovingValidationSystem))]
    public partial struct ControlSystem : ISystem, ISystemStartStop
    {
        private EntityQuery _controllableQ;
        private EntityQuery _keyboardQ;
        private Entity _keyboard;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Input_Keyboard_Up, Input_Keyboard_Down, Input_Keyboard_Left, Input_Keyboard_Right, Input_Keyboard_Cancel, Input_Keyboard_Confirm>();
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
        public void OnStartRunning(ref SystemState state)
        {
            _keyboard = _keyboardQ.GetSingletonEntity();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            _keyboard = Entity.Null;
        }

        [BurstCompile]
        partial struct ControlJob : IJobEntity
        {
            public bool up, down, left, right, cancel, confirm;
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
            var em = state.WorldUnmanaged.EntityManager;
            var up = em.GetComponentData<Input_Keyboard_Up>(_keyboard).triggered;
            var down = em.GetComponentData<Input_Keyboard_Down>(_keyboard).triggered;
            var left = em.GetComponentData<Input_Keyboard_Left>(_keyboard).triggered;
            var right = em.GetComponentData<Input_Keyboard_Right>(_keyboard).triggered;
            var cancel = em.GetComponentData<Input_Keyboard_Cancel>(_keyboard).triggered;
            var confirm = em.GetComponentData<Input_Keyboard_Confirm>(_keyboard).triggered;

            new ControlJob()
            {
                up = up,
                down = down,
                left = left,
                right = right,
                cancel = cancel,
                confirm = confirm
            }.ScheduleParallel(_controllableQ, state.Dependency).Complete();

            em.SetComponentData<Input_Keyboard_Up>(_keyboard, default);
            em.SetComponentData<Input_Keyboard_Down>(_keyboard, default);
            em.SetComponentData<Input_Keyboard_Left>(_keyboard, default);
            em.SetComponentData<Input_Keyboard_Right>(_keyboard, default);
            em.SetComponentData<Input_Keyboard_Cancel>(_keyboard, default);
            em.SetComponentData<Input_Keyboard_Confirm>(_keyboard, default);
        }
    }
}

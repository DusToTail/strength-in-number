using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(Input_Reset_SystemGroup))]
    public partial struct Input_ResetSystem : ISystem, ISystemStartStop
    {
        private EntityQuery _inputQuery;
        private Entity _input;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            InputUtils.GetInputQuery(state.EntityManager, out _inputQuery);

            state.RequireForUpdate(_inputQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _inputQuery.Dispose();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            // Cant use GetSingletonEntity due to Input_Disabled_Flag : IEnableComponent
            var arr = _inputQuery.ToEntityArray(Allocator.Temp);
            _input = arr[0];
            arr.Dispose();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            state.WorldUnmanaged.EntityManager.DestroyEntity(_input);
            _input = Entity.Null;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            em.SetComponentData(_input, new Input_Mouse_Position()
            {
                position = float2.zero
            });
            em.SetComponentData(_input, new Input_Mouse_Select()
            {
                triggered = false
            });
            em.SetComponentData(_input, new Input_Mouse_Deselect()
            {
                triggered = false
            });
            em.SetComponentData(_input, new Input_Keyboard_Confirm()
            {
                triggered = false
            });
            em.SetComponentData(_input, new Input_Keyboard_Cancel()
            {
                triggered = false
            });
            em.SetComponentData(_input, new Input_Keyboard_Up()
            {
                triggered = false
            });
            em.SetComponentData(_input, new Input_Keyboard_Down()
            {
                triggered = false
            });
            em.SetComponentData(_input, new Input_Keyboard_Left()
            {
                triggered = false
            });
            em.SetComponentData(_input, new Input_Keyboard_Right()
            {
                triggered = false
            });
        }
    }
}

using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class ResetPlayerInputSystem : SystemBase
    {
        private Entity _player;

        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
        }

        protected override void OnStartRunning()
        {
            _player = SystemAPI.GetSingletonEntity<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            SystemAPI.SetSingleton(new PlayerMouse()
            {
                position = float2.zero
            });

            SystemAPI.SetSingleton(new PlayerKeyboard()
            {
                wasd = float2.zero
            });
            SystemAPI.SetComponentEnabled<PlayerSelect>(_player, false);
            SystemAPI.SetComponentEnabled<PlayerDeselect>(_player, false);
        }

        protected override void OnStopRunning()
        {
            _player = Entity.Null;
        }
    }
}

using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class PlayerInputSystem : SystemBase
    {
        private Entity _player;
        private PlayerInput _actions;

        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();

            _actions = new PlayerInput();
        }

        protected override void OnStartRunning()
        {
            _player = SystemAPI.GetSingletonEntity<PlayerTag>();

            _actions.Enable();

            _actions.Gameplay.Select.performed += OnPlayerSelect;
            _actions.Gameplay.Deselect.performed += OnPlayerDeselect;
        }

        protected override void OnUpdate()
        {
            float2 mousePosition = _actions.Gameplay.MousePosition.ReadValue<Vector2>();
            SystemAPI.SetSingleton(new PlayerMouse()
            {
                position = mousePosition
            });

            float2 wasd = _actions.Gameplay.WASD.ReadValue<Vector2>();
            SystemAPI.SetSingleton(new PlayerKeyboard()
            {
                wasd = wasd
            });
        }

        protected override void OnStopRunning()
        {
            _actions.Gameplay.Select.performed -= OnPlayerSelect;
            _actions.Gameplay.Deselect.performed -= OnPlayerDeselect;

            _actions.Disable();

            _player = Entity.Null;
        }

        private void OnPlayerSelect(InputAction.CallbackContext obj)
        {
            if (!SystemAPI.Exists(_player))
            {
                return;
            }

            SystemAPI.SetComponentEnabled<PlayerSelect>(_player, true);

            Debug.Log("Select");
        }
        private void OnPlayerDeselect(InputAction.CallbackContext obj)
        {
            if (!SystemAPI.Exists(_player))
            {
                return;
            }

            SystemAPI.SetComponentEnabled<PlayerDeselect>(_player, true);

            Debug.Log("Deselect");
        }
    }
}

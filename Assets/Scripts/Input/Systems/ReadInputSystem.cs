using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber.Input
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class ReadInputSystem : SystemBase
    {
        private DefaultInput _asset;

        protected override void OnCreate()
        {
            _asset = new DefaultInput();
            _asset.Input.Enable();
            RequireForUpdate<Mouse>();
            RequireForUpdate<Keyboard>();
        }

        protected override void OnDestroy()
        {
            _asset.Input.Disable();
            _asset = null;
        }

        protected override void OnUpdate()
        {
            ReadMouse();
            ReadKeyboard();
        }

        private void ReadMouse()
        {
            float2 position = _asset.Input.MousePosition.ReadValue<Vector2>();
            float2 delta = _asset.Input.MouseDelta.ReadValue<Vector2>();
            float scroll = _asset.Input.MouseScroll.ReadValue<float>();
            bool select = _asset.Input.MouseSelect.triggered;
            bool deselect = _asset.Input.MouseDeselect.triggered;
            SystemAPI.GetSingletonRW<Mouse>().ValueRW = new Mouse()
            {
                position = position,
                delta = delta,
                scroll = scroll,
                select = select,
                deselect = deselect
            };
        }

        private void ReadKeyboard()
        {
            bool up = _asset.Input.KeyboardUp.triggered;
            bool down = _asset.Input.KeyboardDown.triggered;
            bool left = _asset.Input.KeyboardLeft.triggered;
            bool right = _asset.Input.KeyboardRight.triggered;
            bool confirm = _asset.Input.KeyboardConfirm.triggered;
            bool cancel = _asset.Input.KeyboardCancel.triggered;
            float2 nav = _asset.Input.KeyboardNavigation.ReadValue<Vector2>();
            SystemAPI.GetSingletonRW<Keyboard>().ValueRW = new Keyboard()
            {
                up = up,
                down = down,
                left = left,
                right = right,
                confirm = confirm,
                cancel = cancel,
                navigation = nav
            };
        }
    }
}

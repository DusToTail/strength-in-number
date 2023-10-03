using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(Input_Initialization_SystemGroup))]
    public partial class GridBuilder_ReadInputSystem : SystemBase
    {
        private GridBuilder_InputAction _actions;
        private EntityQuery _inputQuery;
        private Entity _input;
        private EntityQuery _builderQuery;
        private Entity _builder;

        protected override void OnCreate()
        {
            GridBuilderUtils.GetInputQuery(EntityManager, out _inputQuery);
            GridBuilderUtils.GetBuilderQuery(EntityManager, out _builderQuery);

            RequireForUpdate(_inputQuery);
            RequireForUpdate(_builderQuery);
        }

        protected override void OnDestroy()
        {
            _inputQuery.Dispose();
        }

        protected override void OnStartRunning()
        {
            // Cant use GetSingletonEntity due to Input_Disabled_Flag : IEnableComponent
            var arr = _inputQuery.ToEntityArray(Allocator.Temp);
            _input = arr[0];
            arr.Dispose();

            _builder = _builderQuery.GetSingletonEntity();

            _actions = new GridBuilder_InputAction();
            _actions.GridBuilder.Enable();

            _actions.GridBuilder.MouseSelect.performed      += OnMouseSelect;
            _actions.GridBuilder.MouseDeselect.performed    += OnMouseDeselect;
            _actions.GridBuilder.Confirm.performed          += OnConfirm;
            _actions.GridBuilder.Cancel.performed           += OnCancel;
            _actions.GridBuilder.Up.performed               += OnUp;
            _actions.GridBuilder.Down.performed             += OnDown;
            _actions.GridBuilder.Left.performed             += OnLeft;
            _actions.GridBuilder.Right.performed            += OnRight;
        }

        protected override void OnStopRunning()
        {
            if(_actions.GridBuilder.enabled)
            {
                _actions.GridBuilder.MouseSelect.performed      -= OnMouseSelect;
                _actions.GridBuilder.MouseDeselect.performed    -= OnMouseDeselect;
                _actions.GridBuilder.Confirm.performed          -= OnConfirm;
                _actions.GridBuilder.Cancel.performed           -= OnCancel;
                _actions.GridBuilder.Up.performed               -= OnUp;
                _actions.GridBuilder.Down.performed             -= OnDown;
                _actions.GridBuilder.Left.performed             -= OnLeft;
                _actions.GridBuilder.Right.performed            -= OnRight;

                _actions.GridBuilder.Disable();
            }
            _actions = null;

            EntityManager.DestroyEntity(_input);
            _input = Entity.Null;
            _builder = Entity.Null;
        }

        protected override void OnUpdate()
        {
            if(SystemAPI.IsComponentEnabled<Input_Disabled_Flag>(_input) &&
                _actions.GridBuilder.enabled)
            {
                // If (Disable flag is enabled (meaning _actions should be disabled) AND _actions is enabled)
                // Disable _actions
                _actions.GridBuilder.Disable();
            }
            else if (!SystemAPI.IsComponentEnabled<Input_Disabled_Flag>(_input) &&
                !_actions.GridBuilder.enabled)
            {
                // If (Disable flag is not enabled (meaning _actions should be enabled) AND _actions is disabled)
                // Enable _actions
                _actions.GridBuilder.Enable();
            }

            float2 mousePosition = _actions.GridBuilder.MousePosition.ReadValue<Vector2>();
            SystemAPI.SetComponent(_input, new Input_Mouse_Position()
            {
                position = mousePosition
            });
            //Debug.Log($"ReadInput: Mouse Position {mousePosition}");
        }

        private void OnMouseSelect(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Mouse_Select()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Mouse Select");
        }
        private void OnMouseDeselect(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Mouse_Deselect()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Mouse Deselect");
        }
        private void OnConfirm(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Keyboard_Confirm()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Keyboard Confirm");
        }
        private void OnCancel(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Keyboard_Cancel()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Keyboard Cancel");
        }
        private void OnUp(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Keyboard_Up()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Keyboard Up");
        }
        private void OnDown(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Keyboard_Down()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Keyboard Down");
        }
        private void OnLeft(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Keyboard_Left()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Keyboard Left");
        }
        private void OnRight(InputAction.CallbackContext obj)
        {
            SystemAPI.SetComponent(_input, new Input_Keyboard_Right()
            {
                triggered = true
            });
            Debug.Log("ReadInput: Keyboard Right");
        }
    }
}

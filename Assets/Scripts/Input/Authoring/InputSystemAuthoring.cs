using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber
{
    public class InputSystemAuthoring : MonoBehaviour
    {
        public class InputSystemBaker : Baker<InputSystemAuthoring>
        {
            public override void Bake(InputSystemAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);

                AddComponent(self, typeof(Input_Disabled_Flag));
                SetComponentEnabled<Input_Disabled_Flag>(self, false);

                AddComponent(self, new ComponentTypeSet(
                    typeof(Input_Mouse_Position),
                    typeof(Input_Mouse_Select), typeof(Input_Mouse_Deselect)));

                AddComponent(self, new ComponentTypeSet(
                    typeof(Input_Keyboard_Left), typeof(Input_Keyboard_Right), typeof(Input_Keyboard_Up), typeof(Input_Keyboard_Down)));

                AddComponent(self, new ComponentTypeSet(
                    typeof(Input_Keyboard_Confirm), typeof(Input_Keyboard_Cancel)));
            }
        }
    }
    public struct Input_Mouse_Position : IComponentData
    {
        public float2 position;
    }
    public struct Input_Mouse_Select : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Mouse_Deselect : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Keyboard_Confirm : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Keyboard_Cancel : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Keyboard_Up : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Keyboard_Down : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Keyboard_Left : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Keyboard_Right : IComponentData
    {
        public bool triggered;
    }
    public struct Input_Disabled_Flag : IComponentData, IEnableableComponent
    {
    }
}

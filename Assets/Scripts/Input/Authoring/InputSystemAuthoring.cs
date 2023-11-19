using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber.Input
{
    public class InputSystemAuthoring : MonoBehaviour
    {
        public class InputSystemBaker : Baker<InputSystemAuthoring>
        {
            public override void Bake(InputSystemAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);
                AddComponent(self, new Mouse());
                AddComponent(self, new Keyboard());
            }
        }
    }
    public struct Mouse : IComponentData
    {
        public float2 position;
        public float2 delta;
        public float scroll;
        public bool select;
        public bool deselect;
    }
    public struct Keyboard : IComponentData
    {
        public bool confirm;
        public bool cancel;
        public bool up;
        public bool down;
        public bool left;
        public bool right;
        public float2 navigation;
    }
}

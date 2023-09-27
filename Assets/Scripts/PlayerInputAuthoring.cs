using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber
{
    public class PlayerInputAuthoring : MonoBehaviour
    {
        public class PlayerInputBaker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);

                AddComponent(self, new PlayerTag());
                AddComponent(self, new PlayerMouse()
                {
                    position = float2.zero
                });
                AddComponent(self, new PlayerKeyboard()
                {
                    wasd = float2.zero
                });
                AddComponent(self, new PlayerSelect());
                AddComponent(self, new PlayerDeselect());

            }
        }
    }

    public struct PlayerTag : IComponentData
    {
    }

    public struct PlayerMouse : IComponentData
    {
        public float2 position;
    }

    public struct PlayerKeyboard : IComponentData
    {
        public float2 wasd;
    }

    public struct PlayerSelect : IComponentData, IEnableableComponent
    {
    }

    public struct PlayerDeselect : IComponentData, IEnableableComponent
    {
    }
}

using UnityEngine;
using Unity.Entities;

namespace StrengthInNumber.GridBuilder
{
    public class PlatformDebugAuthoring : MonoBehaviour
    {
        public class PlatformDebugBaker : Baker<PlatformDebugAuthoring>
        {
            public override void Bake(PlatformDebugAuthoring authoring)
            {
                // PlatformDebugSystem is still VERY SLOW. so avoid using this
                var self = GetEntity(TransformUsageFlags.None);
                AddComponent(self, new PlatformDataDebug
                {
                });
            }
        }
    }

    public struct PlatformDataDebug : IComponentData
    {
    }
}

using Unity.Entities;
using UnityEngine;

namespace StrengthInNumber.Gameplay
{
    public class ControllableAuthoring : MonoBehaviour
    {
        public class ControllableBaker : Baker<ControllableAuthoring>
        {
            public override void Bake(ControllableAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);
                AddComponent(self, new ControllableTag());
            }
        }
    }

    public struct ControllableTag : IComponentData
    {
    }
}

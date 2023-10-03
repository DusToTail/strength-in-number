using UnityEngine;
using Unity.Entities;

namespace StrengthInNumber
{
    public class SelectableTagAuthoring : MonoBehaviour
    {
        public class SelectableBaker : Baker<SelectableTagAuthoring>
        {
            public override void Bake(SelectableTagAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                AddComponent(self, new SelectableTag());
                AddComponent(self, new SelectedFlag());
                SetComponentEnabled<SelectedFlag>(self, false);
            }
        }
    }

    public struct SelectableTag : IComponentData
    {
    }
    public struct SelectedFlag : IComponentData, IEnableableComponent
    {
    }
}

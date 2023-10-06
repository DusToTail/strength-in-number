using UnityEngine;
using Unity.Entities;

namespace StrengthInNumber
{
    public class SelectableTagAuthoring : MonoBehaviour
    {
        public bool setSelectable;
        public bool setHoverable;

        public class SelectableBaker : Baker<SelectableTagAuthoring>
        {
            public override void Bake(SelectableTagAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                if(authoring.setSelectable)
                {
                    AddComponent(self, new SelectableTag());
                    AddComponent(self, new SelectedFlag());
                    SetComponentEnabled<SelectedFlag>(self, false);
                }
                
                if(authoring.setHoverable)
                {
                    AddComponent(self, new HoverableTag());
                    AddComponent(self, new HoverredFlag());
                    SetComponentEnabled<HoverredFlag>(self, false);
                }
            }
        }
    }

    public struct SelectableTag : IComponentData
    {
    }
    public struct SelectedFlag : IComponentData, IEnableableComponent
    {
    }
    public struct HoverableTag : IComponentData
    {
    }
    public struct HoverredFlag : IComponentData, IEnableableComponent
    {
    }
}

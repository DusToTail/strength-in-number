using UnityEngine;
using Unity.Entities;

namespace StrengthInNumber
{
    public class InteractionAuthoring : MonoBehaviour
    {
        public bool select;
        public bool hover;

        public class InteractionBaker : Baker<InteractionAuthoring>
        {
            public override void Bake(InteractionAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                if(authoring.select)
                {
                    AddComponent(self, new Selected());
                    SetComponentEnabled<Selected>(self, false);
                }
                
                if(authoring.hover)
                {
                    AddComponent(self, new Hoverred());
                    SetComponentEnabled<Hoverred>(self, false);
                }
            }
        }
    }

    public struct Selected : IComponentData, IEnableableComponent
    {
    }
    public struct Hoverred : IComponentData, IEnableableComponent
    {
    }
}

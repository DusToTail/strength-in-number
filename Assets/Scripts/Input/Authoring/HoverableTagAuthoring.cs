using UnityEngine;
using Unity.Entities;

namespace StrengthInNumber
{
    public class HoverableTagAuthoring : MonoBehaviour
    {
        public class HoverableBaker : Baker<HoverableTagAuthoring>
        {
            public override void Bake(HoverableTagAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                
            }
        }
    }

    
}

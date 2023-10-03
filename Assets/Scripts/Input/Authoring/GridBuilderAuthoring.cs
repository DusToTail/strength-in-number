using Unity.Entities;
using UnityEngine;

namespace StrengthInNumber
{
    public class GridBuilderAuthoring : MonoBehaviour
    {
        public class GridBuilderBaker : Baker<GridBuilderAuthoring>
        {
            public override void Bake(GridBuilderAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);

                AddComponent(self, typeof(Input_GridBuilder_Tag));
                //TODO: more components
            }
        }
    }

    public struct Input_GridBuilder_Tag : IComponentData
    {
    }
}

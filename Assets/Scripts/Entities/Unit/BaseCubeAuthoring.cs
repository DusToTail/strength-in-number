using Unity.Entities;
using UnityEngine;
using StrengthInNumber.Grid;

namespace StrengthInNumber.Entities
{
    public partial class BaseCubeAuthoring : EntityAuthoring
    {
        [HideInInspector]
        public SquareGridAuthoring squareGrid;

        public class BaseCubeBaker : Baker<BaseCubeAuthoring>
        {
            public override void Bake(BaseCubeAuthoring authoring)
            {
                var self = GetEntity(
                    TransformUsageFlags.Renderable |
                    TransformUsageFlags.WorldSpace |
                    TransformUsageFlags.Dynamic);
            }
        }
    }
}

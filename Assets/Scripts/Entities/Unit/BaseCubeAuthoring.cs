using Unity.Entities;
using UnityEngine;
using StrengthInNumber.Grid;

namespace StrengthInNumber.Entities
{
    [DefaultExecutionOrder(0)]
    public partial class BaseCubeAuthoring : EntityAuthoring
    {
        [HideInInspector]
        public SquareGridAuthoring squareGrid;
        public SquareGridUtils.Faces forward;

        public class BaseCubeBaker : EntityBaker<BaseCubeAuthoring>
        {
            public override void Bake(BaseCubeAuthoring authoring)
            {
                base.Bake(authoring);
                AddComponent(self, new Cube()
                {
                    forward = authoring.forward
                });
            }
        }
    }

    public struct Cube : IComponentData
    {
        public SquareGridUtils.Faces forward;
    }
}

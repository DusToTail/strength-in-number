using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber.Entities
{
    public abstract partial class EntityAuthoring : MonoBehaviour
    {
        public Vector2Int position;
        public int size;
        public Mesh mesh;

        public abstract class EntityBaker<T> : Baker<T> where T : EntityAuthoring
        {
            protected Entity self;
            public override void Bake(T authoring)
            {
                self = GetEntity(
                   TransformUsageFlags.Renderable |
                   TransformUsageFlags.WorldSpace |
                   TransformUsageFlags.Dynamic);
                AddComponent(self, new GridPosition()
                {
                    position = new int2(authoring.position.x, authoring.position.y)
                });
            }
        }
    }

    public struct GridPosition : IComponentData
    {
        public int2 position;
    }
}

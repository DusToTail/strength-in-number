using UnityEngine;
using Unity.Entities;
using Unity.Physics;

using Utils = StrengthInNumber.RaycastUtils;

namespace StrengthInNumber
{
    public class MouseRaycastAuthoring : MonoBehaviour
    {
        public float castDistance = 100f;
        public CollisionLayers belongsTo;
        public CollisionLayers collidesWith;

        public class MouseRaycastBaker : Baker<MouseRaycastAuthoring>
        {
            public override void Bake(MouseRaycastAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);

                AddComponent(self, new MouseRaycast()
                {
                    castDistance = authoring.castDistance,
                    filter = new CollisionFilter
                    {
                        BelongsTo = Utils.ToUInt(authoring.belongsTo),
                        CollidesWith = Utils.ToUInt(authoring.collidesWith),
                        GroupIndex = 0
                    },
                    hit = Entity.Null
                });
            }
        }
    }

    public struct MouseRaycast : IComponentData, IEnableableComponent
    {
        public float castDistance;
        public CollisionFilter filter;
        public Entity hit;
    }
}

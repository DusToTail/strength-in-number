using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;

using Utils = StrengthInNumber.RaycastUtils;
using RaycastHit = Unity.Physics.RaycastHit;

namespace StrengthInNumber.Input
{
    public class RaycastAuthoring : MonoBehaviour
    {
        public CollisionLayers belongsTo;
        public CollisionLayers collidesWith;

        public class RaycastBaker : Baker<RaycastAuthoring>
        {
            public override void Bake(RaycastAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);

                AddComponent(self, new RaycastInput()
                {
                    filter = new CollisionFilter
                    {
                        BelongsTo = Utils.ToUInt(authoring.belongsTo),
                        CollidesWith = Utils.ToUInt(authoring.collidesWith),
                        GroupIndex = 0
                    }
                });
                AddComponent(self, new RaycastOutput());
            }
        }
    }

    public struct RaycastInput : IComponentData
    {
        public float3 start;
        public float3 end;
        public CollisionFilter filter;

        public static implicit operator Unity.Physics.RaycastInput(RaycastInput self)
        {
            return new Unity.Physics.RaycastInput()
            {
                Start = self.start,
                End = self.end,
                Filter = self.filter
            };
        }
    }
    public struct RaycastOutput : IComponentData
    {
        public RaycastHit hit;
    }
}

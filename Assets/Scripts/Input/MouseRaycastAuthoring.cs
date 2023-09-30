using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using System;

namespace StrengthInNumber
{
    [Flags]
    public enum CollisionLayers
    {
        Nothing = 0,
        Selection = 1,
        Ground = 1 << 1,
        Object = 1 << 2
    }

    public class MouseRaycastAuthoring : MonoBehaviour
    {
        public float castDistance = 100f;
        public CollisionLayers belongsTo;
        public CollisionLayers collidesWith;

        public static readonly CollisionLayers AllCollisionLayers = 
            CollisionLayers.Nothing |
            CollisionLayers.Selection |
            CollisionLayers.Ground |
            CollisionLayers.Object;

        public class MouseRaycastBaker : Baker<MouseRaycastAuthoring>
        {
            public override void Bake(MouseRaycastAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);

                AddComponent(self, new MouseRaycastData()
                {
                    castDistance = authoring.castDistance,
                    filter = new CollisionFilter
                    {
                        BelongsTo = (uint) (authoring.belongsTo & AllCollisionLayers),
                        CollidesWith = (uint) (authoring.collidesWith & AllCollisionLayers),
                        GroupIndex = 0
                    },
                    hit = Entity.Null
                });
            }
        }
    }

    public struct MouseRaycastData : IComponentData
    {
        public float castDistance;
        public CollisionFilter filter;
        public Entity hit;
    }
}

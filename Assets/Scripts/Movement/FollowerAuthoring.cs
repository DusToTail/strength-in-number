using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber.Movement
{
    public class FollowerAuthoring : MonoBehaviour
    {
        public class FollowerBaker : Baker<FollowerAuthoring>
        {
            public override void Bake(FollowerAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(self, new Follower {
                    target = 0f
                });
            }
        }
    }

    public struct Follower : IComponentData
    {
        public float3 target;
    }
}

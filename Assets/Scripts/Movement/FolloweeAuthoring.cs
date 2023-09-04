using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber.Movement
{
    public class FolloweeAuthoring : MonoBehaviour
    {
        public class FolloweeBaker : Baker<FolloweeAuthoring>
        {
            public override void Bake(FolloweeAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(self, new Followee {
                });
            }
        }
    }

    public struct Followee : IComponentData
    {
    }
}

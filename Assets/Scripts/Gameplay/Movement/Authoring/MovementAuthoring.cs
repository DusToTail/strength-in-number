using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using StrengthInNumber.Entities;
using StrengthInNumber.Grid;

namespace StrengthInNumber.Gameplay
{
    [RequireComponent(typeof(EntityAuthoring))]
    public class MovementAuthoring : MonoBehaviour
    {
        public float speed;

        public class MovementBaker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                var self = GetEntity(
                   TransformUsageFlags.WorldSpace |
                   TransformUsageFlags.Dynamic);

                AddComponent(self, new Movement()
                {
                    fromPosition = authoring.transform.position,
                    toPosition = authoring.transform.position,
                    fromRotation = authoring.transform.rotation,
                    speed = authoring.speed
                });

                AddComponent(self, new IsMoving());
                SetComponentEnabled<IsMoving>(self, false);
            }
        }
    }

    public struct Movement : IComponentData
    {
        public float3 fromPosition;
        public float3 toPosition;
        public quaternion fromRotation;
        public float speed;
        public float lerp;
    }

    public struct IsMoving : IComponentData, IEnableableComponent
    {
    }
}

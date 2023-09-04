using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber.Movement
{
    public class TranslationAuthoring : MonoBehaviour
    {
        public TranslationSettingsSO settings;

        public class MovementBaker : Baker<TranslationAuthoring>
        {
            public override void Bake(TranslationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Translation
                {
                    velocity = authoring.settings.initialVelocity,
                    acceleration = authoring.settings.initialAcceleration,

                    maxSpeed = authoring.settings.maxSpeed,
                    maxAcceleration = authoring.settings.maxAcceleration,
                    dampFactor = authoring.settings.dampFactor
                });
            }
        }
    }

    public struct Translation : IComponentData
    {
        public float3 velocity;
        public float3 acceleration;

        public float maxSpeed;
        public float maxAcceleration;
        public float dampFactor;
    }
}

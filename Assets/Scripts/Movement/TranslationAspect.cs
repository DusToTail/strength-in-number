using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace StrengthInNumber.Movement
{
    public readonly partial struct TranslationAspect : IAspect
    {
        public readonly Entity Self;

        readonly RefRW<LocalTransform> TransformRef;
        readonly RefRW<Translation> TranslationRef;

        public float3 Position
        {
            get { return TransformRef.ValueRO.Position; }
            set { TransformRef.ValueRW.Position = value; }
        }

        public float3 Velocity
        {
            get { return TranslationRef.ValueRO.velocity; }
            set { TranslationRef.ValueRW.velocity = TranslationUtils.ClampMax(value, MaxSpeed); }
        }
        public float3 Acceleration
        {
            get { return TranslationRef.ValueRO.acceleration; }
            set { TranslationRef.ValueRW.acceleration = TranslationUtils.ClampMax(value, MaxAcceleration); }
        }
        

        public float MaxSpeed
        {
            get { return TranslationRef.ValueRO.maxSpeed; }
        }
        public float MaxAcceleration
        {
            get { return TranslationRef.ValueRO.maxAcceleration; }
        }
        public float Damp
        {
            get { return TranslationRef.ValueRO.dampFactor; }
        }

        public void UpdateState(float dt)
        {
            Velocity += Acceleration * dt;
            Velocity *= 1 - Damp * dt;
            Position += Velocity * dt;
        }
    }
}

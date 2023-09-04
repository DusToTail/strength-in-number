using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace StrengthInNumber.Movement
{
    public readonly partial struct SteerAspect : IAspect
    {
        public readonly Entity Self;
        readonly RefRW<LocalTransform> TransformRef;
        readonly RefRW<Translation> TranslationRef;
        readonly RefRW<Follower> FollowRef;
        public float3 Target
        {
            get { return FollowRef.ValueRO.target; }
            set { FollowRef.ValueRW.target = value; }
        }
        public float3 Position
        {
            get { return TransformRef.ValueRO.Position; }
        }
        public float3 Velocity
        {
            get { return TranslationRef.ValueRO.velocity; }
        }
        public float3 Acceleration
        {
            get { return TranslationRef.ValueRO.acceleration; }
            set { TranslationRef.ValueRW.acceleration = TranslationUtils.ClampMax(value, MaxAcceleration); }
        }
        public float MaxAcceleration
        {
            get { return TranslationRef.ValueRO.maxAcceleration; }
        }
        public float MaxSpeed
        {
            get { return TranslationRef.ValueRO.maxSpeed; }
        }
        public float Damp
        {
            get { return TranslationRef.ValueRO.dampFactor; }
        }

        public void UpdateState(float dt)
        {
            float3 desiredVelocity = Target - Position;
            float3 desiredAcceleration = math.normalize(desiredVelocity - Velocity) * MaxAcceleration;

            Acceleration += desiredAcceleration;
            Acceleration *= Damp;
        }
    }
}

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace StrengthInNumber.Gameplay
{
    public readonly partial struct MoveAspect : IAspect
    {
        public readonly Entity Self;

        readonly RefRW<LocalTransform> TransformRW;
        readonly RefRW<Movement> MovementRW;

        public float3 FromPosition
        {
            get { return MovementRW.ValueRO.fromPosition; }
            set { MovementRW.ValueRW.fromPosition = value; }
        }

        public float3 ToPosition
        {
            get { return MovementRW.ValueRO.toPosition; }
            set { MovementRW.ValueRW.toPosition = value; }
        }

        public quaternion FromRotation
        {
            get { return MovementRW.ValueRO.fromRotation; }
            set { MovementRW.ValueRW.fromRotation = value; }
        }

        public float Speed
        {
            get { return MovementRW.ValueRO.speed; }
            set { MovementRW.ValueRW.speed = value; }
        }

        public float Lerp
        {
            get { return MovementRW.ValueRO.lerp; }
            set { MovementRW.ValueRW.lerp = value; }
        }

        public void Move(float dt, float rotateRadius, float rotateAngle)
        {
            Lerp = math.clamp(Lerp + dt * Speed, 0f, 1f);

            float2 fromPos = FromPosition.xz;
            float2 toPos = ToPosition.xz;
            float2 dir = toPos - fromPos;
            if (dir.Equals(0f))
            {
                return;
            }
            float2 xz = math.lerp(fromPos, toPos, Lerp);
            float y = rotateRadius * math.sin(math.radians(90f) + rotateAngle * (Lerp - 0.5f));
            TransformRW.ValueRW.Position = new float3(xz.x, y, xz.y);

            float3 axis = math.cross(new float3(0f, 1f, 0f), new float3(dir.x, 0f, dir.y));
            quaternion rot = quaternion.AxisAngle(axis, rotateAngle);
            quaternion fromRot = FromRotation;
            quaternion toRot = math.mul(rot, fromRot);
            TransformRW.ValueRW.Rotation = math.slerp(fromRot, toRot, Lerp);
        }

        public void Reset()
        {
            Lerp = 0f;
            FromPosition = TransformRW.ValueRO.Position;
            FromRotation = TransformRW.ValueRO.Rotation;
        }
    }
}

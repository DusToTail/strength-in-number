using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

namespace StrengthInNumber.Movement
{
    [UpdateBefore(typeof(TranslationSystem))]
    [BurstCompile]
    public partial struct FollowTargetSystem : ISystem
    {
        private FollowTargetJob _followTargetJob;
        private EntityQuery _followerQuery;
        private EntityQuery _followeeQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _followTargetJob = new FollowTargetJob()
            {
                deltaTime = 0f
            };
            _followerQuery = SystemAPI.QueryBuilder()
                .WithAspect<TranslationAspect>()
                .WithAspect<SteerAspect>()
                .Build();
            _followeeQuery = SystemAPI.QueryBuilder()
                .WithAll<Followee>()
                .Build();
            state.RequireForUpdate(_followerQuery);
            state.RequireForUpdate(_followeeQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var followees = _followeeQuery.ToEntityArray(Allocator.Temp);
            var targets = new NativeArray<float3>(followees.Length, Allocator.TempJob);
            for (int i = 0; i < followees.Length; i++)
            {
                targets[i] = SystemAPI.GetComponentRO<LocalTransform>(followees[i]).ValueRO.Position;
            }
            var findJob = new FindTargetJob()
            {
                targets = targets
            };
            var findJobHandle = findJob.ScheduleParallel(_followerQuery, state.Dependency);
            findJobHandle.Complete();
            followees.Dispose();
            targets.Dispose();


            float dt = SystemAPI.Time.DeltaTime;
            ref var followJob = ref _followTargetJob;
            followJob.deltaTime = dt;
            state.Dependency = followJob.ScheduleParallelByRef(_followerQuery, findJobHandle);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }

    [BurstCompile]
    partial struct FindTargetJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<float3> targets;

        public void Execute(ref Follower followComponent, in LocalTransform position)
        {
            float distance = float.MaxValue;
            int index = -1;
            for(int i = 0; i < targets.Length; i++)
            {
                float cur = math.lengthsq(targets[i] - position.Position);
                if(cur < distance)
                {
                    distance = cur;
                    index = i;
                }
            }
            followComponent.target = index == -1 ? position.Position : targets[index];
        }
    }

    [BurstCompile]
    partial struct FollowTargetJob : IJobEntity
    {
        public float deltaTime;
        public void Execute(SteerAspect steerAspect)
        {
            steerAspect.UpdateState(deltaTime);
        }
    }
}

using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

namespace StrengthInNumber.Movement
{
    [BurstCompile]
    public partial struct TranslationSystem : ISystem
    {
        private TranslationJob _translationJob;
        private EntityQuery _translationQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _translationJob = new TranslationJob()
            {
                deltaTime = 0f
            };
            _translationQuery = SystemAPI.QueryBuilder()
                .WithAspect<TranslationAspect>()
                .Build();
            state.RequireForUpdate(_translationQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            ref var job = ref _translationJob;
            job.deltaTime = dt;
            state.Dependency = job.ScheduleParallelByRef(_translationQuery, state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }

    [BurstCompile]
    partial struct TranslationJob : IJobEntity
    {
        public float deltaTime;
        public void Execute(TranslationAspect translation)
        {
            translation.UpdateState(deltaTime);
        }
    }

    [BurstCompile]
    public static class TranslationUtils
    {
        [BurstCompile]
        public static float3 ClampMax(float3 delta, float max)
        {
            return math.lengthsq(delta) > max * max ?
                math.normalize(delta) * max : delta;
        }
    }
}

using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

namespace StrengthInNumber.GridPlatform
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PlatformCleanupSystem))]
    public partial struct PlatformDebugSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlatformData>();
            state.RequireForUpdate<PlatformDataDebug>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.WorldUnmanaged.EntityManager;
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlatformData>();
            EntityQuery query = builder.Build(em);

            var platforms = query.ToEntityArray(Allocator.Temp);
            builder.Dispose();
            query.Dispose();

            var handles = new NativeArray<JobHandle>(platforms.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for(int i = 0; i < platforms.Length; i++)
            {
                var platform = platforms[i];
                var data = em.GetComponentData<PlatformData>(platform);
                var dcj = new DrawCellJob()
                {
                    positions = data.positions.array,
                    cellWidth = data.cellWidth,
                    cellHeight = data.cellHeight,
                    cellPivot = data.cellPivot
                };

                handles[i] = dcj.Schedule(data.positions.array.Length, 4, state.Dependency);
            }
            JobHandle.CompleteAll(handles);

            handles.Dispose();
            platforms.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public struct DrawCellJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float2> positions;
            public float cellWidth;
            public float cellHeight;
            public PivotPoint cellPivot;

            [BurstCompile]
            public void Execute(int index)
            {
                // NOT PERFORMANT, SLOW!!!!
                float3 _00 = new float3(positions[index].x, 0f, positions[index].y);
                float3 _10 = new float3(positions[index].x, 0f, positions[index].y);
                float3 _01 = new float3(positions[index].x, 0f, positions[index].y);
                float3 _11 = new float3(positions[index].x, 0f, positions[index].y);

                float halfWidth = cellWidth / 2f;
                float halfHeight = cellWidth / 2f;

                if (cellPivot == PivotPoint.BottomLeft)
                {
                    _10 += new float3(2 * halfWidth, 0f, 0f);
                    _01 += new float3(0f, 0f, 2 * halfHeight);
                    _11 += new float3(2 * halfWidth, 0f, 2 * halfHeight);
                }
                else if(cellPivot == PivotPoint.Center)
                {
                    _00 += new float3(-halfWidth, 0f, -halfHeight);
                    _10 += new float3(halfWidth, 0f, -halfHeight);
                    _01 += new float3(-halfWidth, 0f, halfHeight);
                    _11 += new float3(halfWidth, 0f, halfHeight);
                }
                Gizmos.color = Color.yellow;
                Debug.DrawLine(_00, _10, Color.yellow);
                Debug.DrawLine(_10, _11, Color.yellow);
                Debug.DrawLine(_11, _01, Color.yellow);
                Debug.DrawLine(_01, _00, Color.yellow);
            }
        }
    }
}

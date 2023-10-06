using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

using BufferElement = StrengthInNumber.GridBuilder.GridBuilder_GridBufferElement;
using BufferSettings = StrengthInNumber.GridBuilder.GridBuilder_GridBufferSettings;

namespace StrengthInNumber.GridBuilder
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(GridBuilder_CleanupSystem))]
    public partial struct GridBuilder_GridDebugSystem : ISystem, ISystemStartStop
    {
        private EntityQuery _query;
        private BufferLookup<BufferElement> _lookup;
        private Entity _entity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            GridBuilderUtils.GetQuery(state.WorldUnmanaged.EntityManager, out _query);
            GridBuilderUtils.GetBufferLookup(ref state, out _lookup, true);
            state.RequireForUpdate(_query);
            state.RequireForUpdate<GridBuilder_DebugTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _lookup.Update(ref state);
            
            if(_lookup.TryGetBuffer(_entity, out var buffer))
            {
                BufferSettings settings;
                GridBuilderUtils.GetBufferSettings(state.EntityManager, _entity, out settings);
                int length = settings.gridWidth * settings.gridHeight;
                var arr = buffer.ToNativeArray(Allocator.TempJob);
                var dcj = new DrawCellJob()
                {
                    array = arr.AsReadOnly(),
                    cellWidth = settings.cellWidth,
                    cellHeight = settings.cellHeight,
                    color = Color.yellow
                };

                state.Dependency = dcj.Schedule(length, 4, state.Dependency);
                state.CompleteDependency();
                arr.Dispose();
            }
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _entity = _query.GetSingletonEntity();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            _entity = Entity.Null;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _query.Dispose();
        }

        [BurstCompile]
        public struct DrawCellJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<BufferElement>.ReadOnly array;
            public float cellWidth;
            public float cellHeight;
            public Color color;

            [BurstCompile]
            public void Execute(int index)
            {
                // NOT PERFORMANT, SLOW!!!!
                float2 position = array[index].position;
                float halfWidth = cellWidth / 2f;
                float halfHeight = cellHeight / 2f;
                float3 _00 = new float3(position.x - halfWidth, 0f, position.y - halfHeight);
                float3 _10 = new float3(position.x + halfWidth, 0f, position.y - halfHeight);
                float3 _01 = new float3(position.x - halfWidth, 0f, position.y + halfHeight);
                float3 _11 = new float3(position.x + halfWidth, 0f, position.y + halfHeight);
                
                Debug.DrawLine(_00, _10, color);
                Debug.DrawLine(_10, _11, color);
                Debug.DrawLine(_11, _01, color);
                Debug.DrawLine(_01, _00, color);
            }
        }
    }
}

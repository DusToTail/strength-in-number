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
        private EntityQuery _gridBuilderQuery;
        private BufferLookup<BufferElement> _gridLookup;
        private Entity _gridBuilder;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            GridBuilderUtils.GetQuery(state.WorldUnmanaged.EntityManager, out _gridBuilderQuery);
            GridBuilderUtils.GetBufferLookup(ref state, out _gridLookup, true);
            state.RequireForUpdate(_gridBuilderQuery);
            state.RequireForUpdate<GridBuilder_DebugTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _gridLookup.Update(ref state);
            
            if(_gridLookup.TryGetBuffer(_gridBuilder, out var buffer))
            {
                BufferSettings settings;
                GridBuilderUtils.GetBufferSettings(state.EntityManager, _gridBuilder, out settings);
                int length = settings.gridSize.x * settings.gridSize.y;
                var cells = buffer.ToNativeArray(Allocator.TempJob);
                var colors = new NativeArray<Color>(length, Allocator.TempJob);

                for (int y = 0; y < settings.gridSize.y; y++)
                {
                    for(int x = 0; x < settings.gridSize.x; x++)
                    {
                        int index = y * settings.gridSize.x + x;
                        
                        if (buffer[index].selected)
                        {
                            colors[index] = Color.green;
                        }
                        else if (buffer[index].hoverred)
                        {
                            colors[index] = Color.yellow;
                        }
                        else
                        {
                            colors[index] = new Color(1f ,1f, 1f, 0.2f);
                        }

                    }
                }
                var dcj = new DrawCellJob()
                {
                    cells = cells,
                    center = settings.gridCenter,
                    cellSize = settings.cellSize,
                    colors = colors
                };

                dcj.Schedule(length, 4, state.Dependency).Complete();
            }
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _gridBuilder = _gridBuilderQuery.GetSingletonEntity();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            _gridBuilder = Entity.Null;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _gridBuilderQuery.Dispose();
        }

        [BurstCompile]
        public struct DrawCellJob : IJobParallelFor
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BufferElement> cells;
            [DeallocateOnJobCompletion][ReadOnly] public NativeArray<Color> colors;
            public float2 center;
            public float2 cellSize;

            [BurstCompile]
            public void Execute(int index)
            {
                // NOT PERFORMANT, SLOW!!!!
                float2 position = center + cells[index].position;
                float halfWidth = cellSize.x / 2f;
                float halfHeight = cellSize.y / 2f;
                float3 _00 = new float3(position.x - halfWidth, 0f, position.y - halfHeight);
                float3 _10 = new float3(position.x + halfWidth, 0f, position.y - halfHeight);
                float3 _01 = new float3(position.x - halfWidth, 0f, position.y + halfHeight);
                float3 _11 = new float3(position.x + halfWidth, 0f, position.y + halfHeight);
                
                Debug.DrawLine(_00, _10, colors[index]);
                Debug.DrawLine(_10, _11, colors[index]);
                Debug.DrawLine(_11, _01, colors[index]);
                Debug.DrawLine(_01, _00, colors[index]);
            }
        }
    }
}

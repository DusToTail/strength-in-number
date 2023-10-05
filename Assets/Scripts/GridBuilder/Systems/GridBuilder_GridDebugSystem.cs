using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

using UnmanagedGrid = StrengthInNumber.GridBuilder.GridBuilder_UnmanagedGrid;
using Cell = StrengthInNumber.GridBuilder.GridBuilder_UnmanagedGrid.CellData;

namespace StrengthInNumber.GridBuilder
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(GridBuilder_CleanupSystem))]
    public partial struct GridBuilder_GridDebugSystem : ISystem, ISystemStartStop
    {
        private EntityQuery _query;
        private Entity _entity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            GridBuilderUtils.GetQuery(state.WorldUnmanaged.EntityManager, out _query);
            state.RequireForUpdate(_query);
            state.RequireForUpdate<GridBuilder_DebugTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var grid = state.WorldUnmanaged.EntityManager.GetComponentData<UnmanagedGrid>(_entity);

            var dcj = new DrawCellJob()
            {
                cells = grid.cells.array,
                cellWidth = grid.cellWidth,
                cellHeight = grid.cellHeight,
                color = Color.yellow
            };

            state.Dependency = dcj.Schedule(grid.cells.array.Length, 4, state.Dependency);
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
            [ReadOnly] public NativeArray<Cell> cells;
            public float cellWidth;
            public float cellHeight;
            public Color color;

            [BurstCompile]
            public void Execute(int index)
            {
                // NOT PERFORMANT, SLOW!!!!
                float2 position = cells[index].position;
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

using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;

namespace StrengthInNumber.GridBuilder
{
    public class GridBuilderAuthoring : MonoBehaviour
    {
        [Header("Grid")]
        public int xCount;
        public int yCount;

        [Header("Cell")]
        public float cellWidth;
        public float cellHeight;

        [Header("Debug")]
        // TODO: Add flag for adding differnt tags for debugging different parts of the framework
        public bool debug;

        public class GridBuilderBaker : Baker<GridBuilderAuthoring>
        {
            public override void Bake(GridBuilderAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                AddComponent(self, typeof(GridBuilder_MainTag));
                if (authoring.debug)
                {
                    AddComponent(self, typeof(GridBuilder_DebugTag));
                }

                float cellWidth = authoring.cellWidth;
                float cellHeight = authoring.cellHeight;
                int xCount = authoring.xCount;
                int yCount = authoring.yCount;

                // Grid bottom left (0,0) is world origin
                var cells = new NativeArray<GridBuilder_UnmanagedGrid.CellData>(xCount * yCount, Allocator.Persistent);
                float2 offset = new float2(cellWidth, cellHeight) / 2f;
                for (int y = 0; y < yCount; y++)
                {
                    for (int x = 0; x < xCount; x++)
                    {
                        float xPosition = x * cellWidth;
                        float yPosition = y * cellHeight;
                        int index = y * xCount + x;
                        cells[index] = new GridBuilder_UnmanagedGrid.CellData
                        {
                            position = new float2(xPosition, yPosition) + offset
                        };
                    }
                }
                AddComponent(self, new GridBuilder_UnmanagedGrid
                {
                    cellHeight = cellHeight,
                    cellWidth = cellWidth,
                    cells = new Grid2DUnmanaged<GridBuilder_UnmanagedGrid.CellData>(xCount, yCount, cells)
                });
                //TODO: more components
            }
        }
    }

    [ChunkSerializable]
    public struct GridBuilder_UnmanagedGrid : IComponentData, INativeDisposable
    {
        public float cellWidth;
        public float cellHeight;

        public Grid2DUnmanaged<CellData> cells;

        public JobHandle Dispose(JobHandle inputDeps)
        {
            return cells.Dispose(inputDeps);
        }

        public void Dispose()
        {
            cells.Dispose();
        }
        public CellData this[int index]
        {
            get
            {
                return cells[index];
            }
            set
            {
                cells[index] = value;
            }
        }
        public CellData this[int x, int y]
        {
            get
            {
                return cells[x,y];
            }
            set
            {
                cells[x,y] = value;
            }
        }
        public int2 ToGridPosition(float2 worldPosition)
        {
            float xFloat = worldPosition.x / cellWidth;
            float yFloat = worldPosition.y / cellHeight;

            int xInt = math.clamp((int)xFloat, 0, cells.xCount - 1);
            int yInt = math.clamp((int)yFloat, 0, cells.yCount - 1);

            return new int2(xInt, yInt);
        }
        public int ToIndex(float2 worldPosition)
        {
            var gridPosition = ToGridPosition(worldPosition);
            return gridPosition.y * cells.xCount + gridPosition.x;
        }

        public struct CellData
        {
            public float2 position;
        }
    }
    public struct GridBuilder_MainTag : IComponentData
    {
    }
    public struct GridBuilder_DebugTag : IComponentData
    {
    }
}

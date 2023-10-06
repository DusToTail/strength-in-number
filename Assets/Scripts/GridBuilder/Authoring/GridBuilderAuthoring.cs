using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

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
                var self = GetEntity(TransformUsageFlags.None);

                AddComponent(self, typeof(GridBuilder_MainTag));
                if (authoring.debug)
                {
                    AddComponent(self, typeof(GridBuilder_DebugTag));
                }

                float cellWidth = authoring.cellWidth;
                float cellHeight = authoring.cellHeight;
                int xCount = authoring.xCount;
                int yCount = authoring.yCount;
                int length = xCount * yCount;
                AddSharedComponent(self, new GridBuilder_GridBufferSettings()
                {
                    cellWidth = cellWidth,
                    cellHeight = cellHeight,
                    gridWidth = xCount,
                    gridHeight = yCount
                });

                // Grid bottom left (0,0) is world origin
                var cells = new NativeArray<GridBuilder_GridBufferElement>(length, Allocator.Temp);
                float2 offset = new float2(cellWidth, cellHeight) / 2f;
                for (int y = 0; y < yCount; y++)
                {
                    for (int x = 0; x < xCount; x++)
                    {
                        float xPosition = x * cellWidth;
                        float yPosition = y * cellHeight;
                        int index = y * xCount + x;
                        cells[index] = new GridBuilder_GridBufferElement
                        {
                            position = new float2(xPosition, yPosition) + offset
                        };
                    }
                }
               var buffer = AddBuffer<GridBuilder_GridBufferElement>(self);
                buffer.EnsureCapacity(length);
                buffer.AddRange(cells);
                cells.Dispose();
            }
        }
    }
    public struct GridBuilder_MainTag : IComponentData
    {
    }
    public struct GridBuilder_DebugTag : IComponentData
    {
    }
    public struct GridBuilder_GridBufferSettings : ISharedComponentData
    {
        public float cellWidth;
        public float cellHeight;
        public int gridWidth;
        public int gridHeight;
    }
    [InternalBufferCapacity(100)]
    public struct GridBuilder_GridBufferElement : IBufferElementData
    {
        public float2 position;
    }
}

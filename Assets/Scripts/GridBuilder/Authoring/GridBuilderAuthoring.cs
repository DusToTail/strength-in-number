using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics.Authoring;


namespace StrengthInNumber.GridBuilder
{
    public class GridBuilderAuthoring : MonoBehaviour
    {
        [Header("Grid")]
        public int gridWidth;
        public int gridHeight;
        public Vector2 center;
        public PhysicsShapeAuthoring groundCollider;

        [Header("Cell")]
        public float cellWidth;
        public float cellHeight;


        [Header("Debug")]
        // TODO: Add flag for adding differnt tags for debugging different parts of the framework
        public bool debug;
        public bool drawGizmos;

        private void OnValidate()
        {
            if(groundCollider != null)
            {
                groundCollider.SetPlane(
                    new float3(center.x, 0f, center.y),
                    new float2(gridWidth * cellWidth, gridHeight * cellHeight),
                    Quaternion.identity);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if(!drawGizmos)
            {
                return;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, new Vector3(gridWidth * cellWidth, 0.1f, gridHeight * cellHeight));
        }

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
                int gridWidth = authoring.gridWidth;
                int gridHeight = authoring.gridHeight;
                int arrayLength = gridWidth * gridHeight;

                AddSharedComponent(self, new GridBuilder_GridBufferSettings()
                {
                    cellWidth = cellWidth,
                    cellHeight = cellHeight,
                    gridWidth = gridWidth,
                    gridHeight = gridHeight
                });

                var cells = new NativeArray<GridBuilder_GridBufferElement>(arrayLength, Allocator.Temp);
                float2 center = authoring.center;
                float2 offset = new float2(cellWidth, cellHeight) / 2f - new float2(gridWidth * cellWidth, gridHeight * cellHeight) / 2f;
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        float xPosition = x * cellWidth;
                        float yPosition = y * cellHeight;
                        int index = y * gridWidth + x;
                        cells[index] = new GridBuilder_GridBufferElement
                        {
                            position = new float2(xPosition, yPosition) + center + offset
                        };
                    }
                }
               var buffer = AddBuffer<GridBuilder_GridBufferElement>(self);
                buffer.EnsureCapacity(arrayLength);
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

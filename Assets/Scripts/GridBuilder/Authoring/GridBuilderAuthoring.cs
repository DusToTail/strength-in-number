using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using UnityEngine;


namespace StrengthInNumber.GridBuilder
{
    public class GridBuilderAuthoring : MonoBehaviour
    {
        public GridSettings settings;
        public PhysicsShapeAuthoring groundCollider;

        [Header("Debug")]
        // TODO: Add flag for adding differnt tags for debugging different parts of the framework
        public bool debug;
        public bool drawGizmos;

        private void OnValidate()
        {
            if(groundCollider != null)
            {
                groundCollider.SetPlane(
                    new float3(settings.center.x, 0f, settings.center.y),
                    new float2(settings.gridSize.x * settings.cellSize.x, settings.gridSize.y * settings.cellSize.y),
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
            Gizmos.DrawWireCube(
                new Vector3(settings.center.x, 0f, settings.center.y),
                new Vector3(settings.gridSize.x * settings.cellSize.x, 0.1f, settings.gridSize.y * settings.cellSize.y));
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

                float2 cellSize = authoring.settings.cellSize;
                int2 gridSize = new int2(authoring.settings.gridSize.x, authoring.settings.gridSize.y);
                float2 center = authoring.settings.center;

                int arrayLength = gridSize.x * gridSize.y;

                AddSharedComponent(self, new GridBuilder_GridBufferSettings()
                {
                    gridSize = gridSize,
                    gridCenter = center,
                    cellSize = cellSize
                });

                var cells = new NativeArray<GridBuilder_GridBufferElement>(arrayLength, Allocator.Temp);
                float2 offset = cellSize / 2f - new float2(gridSize.x * cellSize.x, gridSize.y * cellSize.y) / 2f;
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int x = 0; x < gridSize.x; x++)
                    {
                        float xPosition = x * cellSize.x;
                        float yPosition = y * cellSize.y;
                        int index = y * gridSize.x + x;
                        cells[index] = new GridBuilder_GridBufferElement
                        {
                            position = new float2(xPosition, yPosition) + center + offset,
                            hoverred = false,
                            selected = false
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
    [Serializable]
    public struct GridSettings
    {
        [Header("Grid")]
        public Vector2Int gridSize;
        public Vector2 center;

        [Header("Cell")]
        public Vector2 cellSize;
    }
    public struct GridBuilder_MainTag : IComponentData
    {
    }
    public struct GridBuilder_DebugTag : IComponentData
    {
    }
    public struct GridBuilder_GridBufferSettings : ISharedComponentData
    {
        public int2 gridSize;
        public float2 gridCenter;
        public float2 cellSize;
    }
    [InternalBufferCapacity(100)]
    public struct GridBuilder_GridBufferElement : IBufferElementData
    {
        public float2 position;
        public bool hoverred;
        public bool selected;
    }
}

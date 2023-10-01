using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace StrengthInNumber.GridPlatform
{
    public enum PivotPoint
    {
        BottomLeft,
        Center
    }

    public class PlatformAuthoring : MonoBehaviour
    {
        public PlatformSettingsSO settings;

        public class PlatformBaker : Baker<PlatformAuthoring>
        {
            public override void Bake(PlatformAuthoring authoring)
            {
                var settings = authoring.settings;
                if(settings == null)
                {
                    settings = new PlatformSettingsSO()
                    {
                        grid = new Grid2DSettingsSO()
                        {
                            xCount = 10,
                            yCount = 10
                        },
                        cellWidth = 1,
                        cellHeight = 1,
                        cellPivot = PivotPoint.Center
                    };
                }
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                var positions = new NativeArray<float2>(settings.grid.xCount * settings.grid.yCount, Allocator.Persistent);

                float2 offset = float2.zero;
                if (settings.cellPivot == PivotPoint.Center)
                {
                    offset = new float2(settings.cellWidth, settings.cellHeight) / 2f;
                }
                for (int y = 0; y < settings.grid.yCount; y++)
                {
                    for (int x = 0; x < settings.grid.xCount; x++)
                    {
                        float xPosition = x * settings.cellWidth;
                        float yPosition = y * settings.cellHeight;
                        int index = y * settings.grid.xCount + x;
                        positions[index] = new float2(xPosition, yPosition) + offset;
                    }
                }
                AddComponent(self, new PlatformData
                {
                    cellHeight = settings.cellHeight,
                    cellWidth = settings.cellWidth,
                    cellPivot = settings.cellPivot,
                    positions = new Grid2DUnmanaged<float2>()
                    {
                        xCount = settings.grid.xCount,
                        yCount = settings.grid.yCount,
                        array = positions
                    }
                });
            }
        }
    }

    [ChunkSerializable]
    public struct PlatformData : IComponentData
    {
        public float cellWidth;
        public float cellHeight;
        public PivotPoint cellPivot;

        public Grid2DUnmanaged<float2> positions;

        public int2 ToGridPosition(float2 worldPosition)
        {
            float xFloat = worldPosition.x / cellWidth;
            float yFloat = worldPosition.y / cellHeight;

            int xInt = math.clamp((int)xFloat, 0, positions.xCount - 1);
            int yInt = math.clamp((int)yFloat, 0, positions.yCount - 1);

            return new int2(xInt, yInt);
        }
        public int ToIndex(float2 worldPosition)
        {
            return positions.ToIndex(ToGridPosition(worldPosition));
        }
    }
}

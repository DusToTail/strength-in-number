using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace StrengthInNumber.GridPlatform
{
    public class PlatformAuthoring : MonoBehaviour
    {
        public PlatformSettingsSO settings;

        public class PlatformBaker : Baker<PlatformAuthoring>
        {
            public override void Bake(PlatformAuthoring authoring)
            {
                var settings = authoring.settings;

                var self = GetEntity(TransformUsageFlags.WorldSpace);

                var positions = new NativeArray<float2>(settings.grid.xCount * settings.grid.yCount, Allocator.Persistent);

                float2 offset = float2.zero;
                if (settings.cell.pivot == PivotPoint.Center)
                {
                    offset = new float2(settings.cell.width, settings.cell.height) / 2f;
                }
                for (int y = 0; y < settings.grid.yCount; y++)
                {
                    for (int x = 0; x < settings.grid.xCount; x++)
                    {
                        float xPosition = x * settings.cell.width;
                        float yPosition = y * settings.cell.height;
                        int index = y * settings.grid.xCount + x;
                        positions[index] = new float2(xPosition, yPosition) + offset;
                    }
                }
                AddComponent(self, new Platform
                {
                    cellHeight = settings.cell.height,
                    cellWidth = settings.cell.width,
                    cellPivot = settings.cell.pivot,
                    Positions = new Grid2DUnmanaged<float2>()
                    {
                        xCount = settings.grid.xCount,
                        yCount = settings.grid.yCount,
                        array = positions
                    }
                });
            }
        }
    }

    public struct Platform : IComponentData
    {
        public float cellWidth;
        public float cellHeight;
        public PivotPoint cellPivot;

        public Grid2DUnmanaged<float2> Positions;

        public int2 ToGridPosition(float2 worldPosition)
        {
            float xFloat = worldPosition.x / cellWidth;
            float yFloat = worldPosition.y / cellHeight;

            int xInt = math.clamp((int)xFloat, 0, Positions.xCount - 1);
            int yInt = math.clamp((int)yFloat, 0, Positions.yCount - 1);

            return new int2(xInt, yInt);
        }
        public int ToIndex(float2 worldPosition)
        {
            return Positions.ToIndex(ToGridPosition(worldPosition));
        }
    }
}

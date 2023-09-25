using UnityEngine;
using Unity.Mathematics;

namespace StrengthInNumber.GridPlatform
{
    public class Platform
    {
        public PlatformSettingsSO Settings { get; private set; }

        public Grid2D<float2> Positions { get; private set; }

        public Platform(PlatformSettingsSO settings)
        {
            Settings = settings;
            Positions = new Grid2D<float2>(Settings.grid);

            float2 offset = float2.zero;
            if(Settings.cell.pivot == PivotPoint.Center)
            {
                offset = new float2(Settings.cell.width, Settings.cell.height) / 2f;
            }
            for (int y = 0; y < Settings.grid.yCount; y++)
            {
                for(int x = 0; x < Settings.grid.xCount; x++)
                {
                    float xPosition = x * Settings.cell.width;
                    float yPosition = y * Settings.cell.height;
                    Positions.Fill(new float2(xPosition, yPosition) + offset, Positions.ToIndex(x, y));
                }
            }
        }

        public int2 ToGridPosition(float2 worldPosition)
        {
            float xFloat = worldPosition.x / Settings.cell.width;
            float yFloat = worldPosition.y / Settings.cell.height;

            int xInt = math.clamp((int)xFloat, 0, Settings.grid.xCount - 1);
            int yInt = math.clamp((int)yFloat, 0, Settings.grid.yCount - 1);

            return new int2(xInt, yInt);
        }
        public int ToIndex(float2 worldPosition)
        {
            return Positions.ToIndex(ToGridPosition(worldPosition));
        }
    }

    [CreateAssetMenu(fileName = "PlatformSettings", menuName = "Custom/PlatformSettings")]
    public class PlatformSettingsSO : ScriptableObject
    {
        public Grid2DSettingsSO grid;
        public CellSettingsSO cell;
    }

    public enum PivotPoint
    {
        BottomLeft,
        Center
    }

    [CreateAssetMenu(fileName = "CellSettings", menuName = "Custom/CellSettings")]
    public class CellSettingsSO : ScriptableObject
    {
        public float width;
        public float height;
        public PivotPoint pivot;
    }
}

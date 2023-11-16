using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber.Grid
{
    [DefaultExecutionOrder(-1000)]
    public partial class SquareGridAuthoring : GridAuthoring
    {
        public class SquareGridBaker : GridBaker<SquareGridAuthoring, SquareGrid, SquareGridElement>
        {
            public override SquareGridElement CreateElement(int x, int y, IGrid grid)
            {
                float2 pos = grid.GridToWorld(x, y).xz;
                return new SquareGridElement
                {
                    Position = new float3(pos.x, grid.Center.y, pos.y),
                    Index = grid.GridToIndex(x,y),
                    Entity = Entity.Null
                };
            }

            public override SquareGrid CreateGrid()
            {
                var offset = new float2(authoring.cellSize / 2f) -
                                new float2(authoring.width, authoring.height) * authoring.cellSize / 2f;
                var grid = new SquareGrid()
                {
                    Width = authoring.width,
                    Height = authoring.height,
                    Center = authoring.center,
                    CellSize = authoring.cellSize,
                    Offset = new float3(offset.x, 0f, offset.y)
                };
                return grid;
            }
        }
    }

    public struct SquareGridElement : IGridElement
    {
        public Entity Entity { get; set; }
        public int Index { get; set; }
        public float3 Position { get; set; }
    }

    public struct SquareGrid : IGrid
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float3 Center { get; set; }
        public float CellSize { get; set; }
        public float3 Offset { get; set; }

        public int GridToIndex(int x, int y)
        {
            return y * Width + x;
        }

        public int2 WorldToGrid(float3 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            float2 diff =   position.xz - 
                            (Center.xz + Offset.xz - new float2(CellSize / 2));
            int x = (int)(diff.x / CellSize);
            int y = (int)(diff.y / CellSize);
            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, Width - 1);
                y = math.clamp(y, 0, Height - 1);
            }

            if (x < 0 || x > Width - 1 ||
                y < 0 || y > Height - 1)
            {
                return new int2(-1);
            }

            return new int2(x, y);
        }

        public int2 IndexToGrid(int index)
        {
            return new int2(index / Width, index % Width);
        }

        public float3 GridToWorld(int x, int y)
        {
            return new float3(x, 0f, y) * CellSize + Center + Offset;
        }

        public static int GridToIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        public static int2 WorldToGrid(float3 position, float3 center, float cellSize, int width, int height, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            float2 offset = new float2(cellSize / 2f) -
                                new float2(width, height) * cellSize / 2f;
            float2 diff = position.xz -
                            (center.xz + offset - new float2(cellSize / 2));
            int x = (int)(diff.x / cellSize);
            int y = (int)(diff.y / cellSize);
            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, width - 1);
                y = math.clamp(y, 0, height - 1);
            }

            if (x < 0 || x > width - 1 ||
                y < 0 || y > height - 1)
            {
                return new int2(-1);
            }

            return new int2(x, y);
        }

        public static int2 IndexToGrid(int index, int width)
        {
            return new int2(index / width, index % width);
        }

        public static float3 GridToWorld(int x, int y, float cellSize, float3 center, int width, int height)
        {
            float2 offset = new float2(cellSize / 2f) -
                                new float2(width, height) * cellSize / 2f;
            return new float3(x, 0f, y) * cellSize + center + new float3(offset.x, 0f, offset.y);
        }
    }
}

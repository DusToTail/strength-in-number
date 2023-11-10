using Unity.Entities;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace StrengthInNumber.Grid
{
    public partial class TriangleGridAuthoring : GridAuthoring
    {
        public class TriangleGridBaker : GridBaker<TriangleGridAuthoring, TriangleGrid, TriangleGridElement>
        {
            public override TriangleGridElement CreateElement(int x, int y, IGrid grid)
            {
                return new TriangleGridElement
                {
                    Position = new float3(
                        x * grid.CellSize / 2f + grid.Center.x + grid.Offset.x,
                        grid.Center.y,
                        AlternateHeightSum(
                            y,
                            ((TriangleGrid)grid).CenterToEdge,
                            ((TriangleGrid)grid).CenterToVertex,
                            (x & 1) == 0) +
                            grid.Center.z + grid.Offset.y
                        ),
                    Index = grid.GridToIndex(x, y),
                    Entity = Entity.Null
                };

                float AlternateHeightSum(int y, float centerToEdge, float centerToVertex, bool evenColumn)
                {
                    float result = ((y / 2) * 2 + 1) * (evenColumn ? centerToEdge : centerToVertex) +
                                   (((y + 1) / 2) * 2) * (evenColumn ? centerToVertex : centerToEdge);
                    return result;
                }
            }

            public override TriangleGrid CreateGrid()
            {
                var grid = new TriangleGrid()
                {
                    Width = authoring.width,
                    Height = authoring.height,
                    Center = authoring.center,
                    CellSize = authoring.cellSize,
                    Offset = new float2(authoring.cellSize / 2f, authoring.cellSize * sqrt(3) / 6) -
                            new float2((authoring.width + 1) * authoring.cellSize * 0.5f, authoring.cellSize * sqrt(3) / 2f * authoring.height) / 2f
                };
                return grid;
            }
        }
    }

    public struct TriangleGridElement : IGridElement
    {
        public Entity Entity { get; set; }
        public int Index { get; set; }
        public float3 Position { get; set; }
    }

    public struct TriangleGrid : IGrid
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float3 Center { get; set; }
        public float CellSize { get; set; }
        public float2 Offset { get ; set ; }

        public float CellHeight { get { return CellSize * sqrt(3) / 2; } }
        public float CenterToVertex { get { return CellSize / sqrt(3); } }
        public float CenterToEdge { get { return CellSize * sqrt(3) / 6; } }

        public int GridToIndex(int x, int y)
        {
            return y * Width + x;
        }

        public int2 WorldToGrid(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            float2 diff =   position - 
                            (Center.xz + Offset - new float2(CellSize / 2f, CenterToEdge));
            float yFloat = diff.y / CellHeight;
            int y = (int)yFloat;
            float xFloat = diff.x / CellSize;
            int x = (int)xFloat;
            float yDecimal = frac(yFloat);
            float xDecimal = frac(xFloat);
            if (xDecimal != 0.5f) // If x does not lie exactly at the middle of the triangle, needs to evaluate for left and right triangles
            {
                if ((y & 1) == 0) // If even (up/down pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = float2(0);
                        float2 B = new float2(CellSize * 0.5f, CellHeight);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(CellSize * 0.5f, CellHeight);
                        float2 B = new float2(CellSize, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
                else // If odd (down/up pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = new float2(0f, CellHeight);
                        float2 B = new float2(CellSize * 0.5f, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(CellSize * 0.5f, 0f);
                        float2 B = new float2(CellSize, CellHeight);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
            }

            if (alwaysInGrid)
            {
                x = clamp(x, 0, Width - 1);
                y = clamp(y, 0, Height - 1);
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

        public float2 GridToWorld(int x, int y)
        {
            float xPosition = x * CellSize / 2f + Center.x + Offset.x;
            float zPosition = AlternateHeightSum(y, (x & 1) == 0)
                + Center.z + Offset.y;
            return new float2(xPosition, zPosition);
        }

        

        public static int GridToIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        public static int2 WorldToGrid(float2 position, float2 center, float cellSize, int width, int height, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            float centerToEdge = cellSize * sqrt(3) / 6;
            float cellHeight = cellSize * sqrt(3) / 2;
            float2 offset = new float2(cellSize / 2f, cellSize * sqrt(3) / 6) -
                            new float2((width + 1) * cellSize * 0.5f, cellSize * sqrt(3) / 2f * height) / 2f;
            float2 diff = position -
                            (center + offset - new float2(cellSize / 2f, centerToEdge));
            float yFloat = diff.y / cellHeight;
            int y = (int)yFloat;
            float xFloat = diff.x / cellSize;
            int x = (int)xFloat;
            float yDecimal = frac(yFloat);
            float xDecimal = frac(xFloat);
            if (xDecimal != 0.5f) // If x does not lie exactly at the middle of the triangle, needs to evaluate for left and right triangles
            {
                if ((y & 1) == 0) // If even (up/down pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = float2(0);
                        float2 B = new float2(cellSize * 0.5f, cellHeight);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(cellSize * 0.5f, cellHeight);
                        float2 B = new float2(cellSize, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
                else // If odd (down/up pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = new float2(0f, cellHeight);
                        float2 B = new float2(cellSize * 0.5f, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(cellSize * 0.5f, 0f);
                        float2 B = new float2(cellSize, cellHeight);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
            }

            if (alwaysInGrid)
            {
                x = clamp(x, 0, width - 1);
                y = clamp(y, 0, height - 1);
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

        public static float2 GridToWorld(int x, int y, float2 center, float cellSize, int width, int height)
        {
            float2 offset = new float2(cellSize / 2f, cellSize * sqrt(3) / 6) -
                new float2((width + 1) * cellSize * 0.5f, cellSize * sqrt(3) / 2f * height) / 2f;
            float xPosition = x * cellSize / 2f + center.x + offset.x;
            float centerToEdge = cellSize * sqrt(3) / 6;
            float centerToVertex = cellSize / sqrt(3);
            float zPosition = AlternateHeightSum(y, centerToEdge, centerToVertex, (x & 1) == 0)
                + center.y + offset.y;
            return new float2(xPosition, zPosition);
        }

        public float AlternateHeightSum(int y, bool evenColumn)
        {
            float result = ((y / 2) * 2 + 1) * (evenColumn ? CenterToEdge : CenterToVertex) +
                           (((y + 1) / 2) * 2) * (evenColumn ? CenterToVertex : CenterToEdge);
            return result;
        }

        public static float AlternateHeightSum(int y, float centerToEdge, float centerToVertex, bool evenColumn)
        {
            float result = ((y / 2) * 2 + 1) * (evenColumn ? centerToEdge : centerToVertex) +
                           (((y + 1) / 2) * 2) * (evenColumn ? centerToVertex : centerToEdge);
            return result;
        }

        public static bool IsFlipTriangle(int x, int y)
        {
            if ((y % 2 == 0 && x % 2 == 1) ||
                (y % 2 == 1 && x % 2 == 0))
            {
                return true;
            }
            return false;
        }
    }
}

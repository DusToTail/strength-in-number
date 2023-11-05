using Unity.Entities;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace StrengthInNumber.Grid
{
    public partial class TriangleGridAuthoring : GridAuthoring
    {
        public class TriangleGridBaker : GridBaker<TriangleGridAuthoring, TriangleGrid, TriangleGridElement>
        {
            private float2 _offset;

            public override TriangleGridElement CreateElement(int x, int y, IGrid grid)
            {
                return new TriangleGridElement
                {
                    Position = new float3(
                        x * grid.CellSize / 2f + grid.Center.x + _offset.x,
                        grid.Center.y,
                        AlternateHeightSum(
                            y,
                            ((TriangleGrid)grid).CenterToEdge,
                            ((TriangleGrid)grid).CenterToVertex,
                            (x & 1) == 0) +
                            grid.Center.z + _offset.y
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
                };

                _offset = new float2(grid.CellSize / 2f, grid.CenterToEdge) -
                    new float2((grid.Width + 1) * grid.CellSize * 0.5f, grid.CellHeight * grid.Height) / 2f;
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

        public float CellHeight { get { return CellSize * sqrt(3) / 2; } }
        public float CenterToVertex { get { return CellSize / sqrt(3); } }
        public float CenterToEdge { get { return CellSize * sqrt(3) / 6; } }

        public bool IsFlipTriangle(int x, int y)
        {
            if ((y % 2 == 0 && x % 2 == 1) ||
                       (y % 2 == 1 && x % 2 == 0))
            {
                return true;
            }
            return false;
        }

        public int WorldToIndex(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            float2 diff = position - Center.xz - new float2((Width + 1) * CellSize * 0.5f, CellHeight * Height) / 2f;
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
                return y * Width + x;
            }

            if (x < 0 || x > Width - 1 ||
                y < 0 || y > Height - 1)
            {
                return -1;
            }
            return y * Width + x;
        }

        public int GridToIndex(int x, int y)
        {
            return y * Width + x;
        }
    }
}

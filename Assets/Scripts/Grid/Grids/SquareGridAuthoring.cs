using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber.Grid
{
    public partial class SquareGridAuthoring : GridAuthoring
    {
        public class SquareGridBaker : GridBaker<SquareGridAuthoring, SquareGrid, SquareGridElement>
        {
            private float2 _offset;

            public override SquareGridElement CreateElement(int x, int y, IGrid grid)
            {
                return new SquareGridElement
                {
                    Position = new float3(
                        x * grid.CellSize + grid.Center.x + _offset.x,
                        grid.Center.y,
                        y * grid.CellSize + grid.Center.z + _offset.y
                        ),
                    Index = grid.GridToIndex(x,y),
                    Entity = Entity.Null
                };
            }

            public override SquareGrid CreateGrid()
            {
                var grid = new SquareGrid()
                {
                    Width = authoring.width,
                    Height = authoring.height,
                    Center = authoring.center,
                    CellSize = authoring.cellSize,
                };

                _offset = new float2(grid.CellSize / 2f) - new float2(grid.Width, grid.Height) * grid.CellSize / 2f;
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

        public int WorldToIndex(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            float2 diff = position - Center.xz - new float2(CellSize * Width, CellSize * Height) / 2;
            int x = (int)(diff.x / CellSize);
            int y = (int)(diff.y / CellSize);
            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, Width - 1);
                y = math.clamp(y, 0, Height - 1);
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

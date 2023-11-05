using Unity.Mathematics;
using Unity.Entities;

namespace StrengthInNumber.Grid
{
    public interface IGrid : ISharedComponentData
    {
        int Width { get; set; }
        int Height { get; set; }
        float3 Center { get; set; }
        float CellSize { get; set; }

        int WorldToIndex(float2 position, bool alwaysInGrid);
        int GridToIndex(int x, int y);
    }

    public interface IGridElement : IBufferElementData
    {
        Entity Entity { get; set; }
        int Index { get; set; }
    }
}

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
        float2 Offset { get; set; }
        int2 WorldToGrid(float2 position, bool alwaysInGrid);
        int GridToIndex(int x, int y);
        int2 IndexToGrid(int index);
        float2 GridToWorld(int x, int y);
    }

    public interface IGridElement : IBufferElementData
    {
        Entity Entity { get; set; }
        int Index { get; set; }
    }
}

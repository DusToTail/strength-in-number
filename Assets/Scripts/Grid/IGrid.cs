using Unity.Mathematics;
using Unity.Entities;

namespace StrengthInNumber.Grid
{
    public interface IGrid : IComponentData
    {
        int Width { get; set; }
        int Height { get; set; }
        float3 Center { get; set; }
        float CellSize { get; set; }
        float3 Offset { get; set; }
        int2 WorldToGrid(float3 position, bool alwaysInGrid);
        int GridToIndex(int x, int y);
        int2 IndexToGrid(int index);
        float3 GridToWorld(int x, int y);
    }

    public interface IGridElement : IBufferElementData
    {
        Entity Entity { get; set; }
        int Index { get; set; }
    }
}

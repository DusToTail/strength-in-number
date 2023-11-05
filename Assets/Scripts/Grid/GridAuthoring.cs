using Unity.Entities;
using Unity.Collections;
using UnityEngine;

namespace StrengthInNumber.Grid
{
    public abstract partial class GridAuthoring : MonoBehaviour
    {
        public int width;
        public int height;
        public Vector3 center;
        public float cellSize;

        public abstract class GridBaker<TAuthoring, UComponent, VBufferElement> : Baker<TAuthoring> 
            where TAuthoring : GridAuthoring
            where UComponent : unmanaged, IGrid
            where VBufferElement : unmanaged, IGridElement
        {
            protected TAuthoring authoring;

            public override void Bake(TAuthoring authoring)
            {
                this.authoring = authoring;
                var self = GetEntity(TransformUsageFlags.WorldSpace);
                var grid = CreateGrid();
                AddSharedComponent(self, grid);
                 
                var cells = new NativeArray<VBufferElement>(grid.Width * grid.Height, Allocator.Temp);

                for (int y = 0; y < grid.Height; y++)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        int index = grid.GridToIndex(x, y);
                        cells[index] = CreateElement(x, y, grid);
                    }
                }

                var buffer = AddBuffer<VBufferElement>(self);
                buffer.EnsureCapacity(cells.Length);
                buffer.AddRange(cells);
                cells.Dispose();
            }

            public abstract UComponent CreateGrid();
            public abstract VBufferElement CreateElement(int x, int y, IGrid grid);
        }

        //public class Grid2DBaker : Baker<GridAuthoring>
        //{
        //    public override void Bake(GridAuthoring authoring)
        //    {
        //        switch(authoring.shape)
        //        {
        //            case Shape.Square:
        //                {
        //                    AddSquareGrid(
        //                        authoring.resolution,
        //                        authoring.center,
        //                        authoring.cellSize,
        //                        authoring.cellPrefab);
        //                    break;
        //                }
        //            case Shape.Triangle:
        //                {
        //                    AddTriangleGrid(
        //                        authoring.resolution,
        //                        authoring.center,
        //                        authoring.cellSize,
        //                        authoring.cellPrefab);
        //                    break;
        //                }
        //        }
        //    }

        //    private void AddSquareGrid(int resolution, float3 center, float cellSize, GameObject prefab)
        //    {
        //        var self = GetEntity(TransformUsageFlags.WorldSpace);

        //        var settings = new SquareGridSettings()
        //        {
        //            CellPrefab = new EntityPrefabReference(prefab),
        //            Resolution = resolution,
        //            Center = center,
        //            CellSize = cellSize,
        //        };
        //        AddSharedComponent(self, settings);

        //        var cells = new NativeArray<GridBufferElementData>(settings.Width * settings.Height, Allocator.Temp);
        //        float2 offset = new float2(cellSize / 2f) - new float2(settings.Width, settings.Height) * cellSize / 2f;
        //        for (int y = 0; y < settings.Height; y++)
        //        {
        //            for (int x = 0; x < settings.Width; x++)
        //            {
        //                float xPosition = x * cellSize + center.x + offset.x;
        //                float yPosition = center.y;
        //                float zPosition = y * cellSize + center.z + offset.y;
        //                int index = y * settings.Width + x;
        //                cells[index] = new GridBufferElementData
        //                {
        //                    position = new float3(xPosition, yPosition, zPosition),
        //                    index = index,
        //                    entity = Entity.Null
        //                };
        //            }
        //        }
        //        // Still need to instatiate the prefab per element in a system
        //        var buffer = AddBuffer<GridBufferElementData>(self);
        //        buffer.EnsureCapacity(cells.Length);
        //        buffer.AddRange(cells);
        //        cells.Dispose();
        //    }

        //    private void AddTriangleGrid(int resolution, float3 center, float cellSize, GameObject prefab)
        //    {
        //        var self = GetEntity(TransformUsageFlags.WorldSpace);

        //        var settings = new TriangleGridSettings()
        //        {
        //            CellPrefab = new EntityPrefabReference(prefab),
        //            Resolution = resolution,
        //            Center = center,
        //            CellSize = cellSize,
        //        };
        //        AddSharedComponent(self, settings);

        //        var cells = new NativeArray<GridBufferElementData>(settings.Width * settings.Height, Allocator.Temp);
        //        float2 offset = new float2(cellSize / 2f, settings.CenterToEdge) - 
        //            new float2((settings.Width + 1) * cellSize * 0.5f, settings.CellHeight * settings.Height) / 2f;
        //        for (int y = 0; y < settings.Height; y++)
        //        {
        //            for (int x = 0; x < settings.Width; x++)
        //            {
        //                float xPosition = x * cellSize / 2f + center.x + offset.x;
        //                float yPosition = center.y;
        //                float zPosition = AlternateHeightSum(y, settings.CenterToEdge, settings.CenterToVertex, (x & 1) == 0) 
        //                    + center.z + offset.y;
        //                int index = y * settings.Width + x;
        //                cells[index] = new GridBufferElementData
        //                {
        //                    position = new float3(xPosition, yPosition, zPosition),
        //                    index = index,
        //                    entity = Entity.Null
        //                };
        //            }
        //        }
        //        // Still need to instatiate the prefab per element in a system
        //        var buffer = AddBuffer<GridBufferElementData>(self);
        //        buffer.EnsureCapacity(cells.Length);
        //        buffer.AddRange(cells);
        //        cells.Dispose();

        //        // Moved to a function for readability
        //        float AlternateHeightSum(int y, float centerToEdge, float centerToVertex, bool evenColumn)
        //        {
        //            float result = ((y / 2) * 2 + 1) * (evenColumn ? centerToEdge : centerToVertex) +
        //                           (((y + 1) / 2) * 2) * (evenColumn ? centerToVertex : centerToEdge);
        //            return result;
        //        }
        //    }
        //}
    }

    //public struct GridBufferElementData : IBufferElementData
    //{
    //    public Entity entity;
    //    public float3 position;
    //    public int index;
    //}

    //public struct SquareGridElement : IComponentData
    //{
    //    public int index;
    //}
    //public struct TriangleGridElement : IComponentData
    //{
    //    public int index;
    //}
    
    //public struct TriangleGridSettings : ISharedComponentData, IGrid
    //{
    //    public EntityPrefabReference CellPrefab;
    //    public const float Root3 = 1.732f;
    //    public const float H = Root3 / 2f;
    //    public float CellHeight { get { return CellSize * H; } }
    //    public float CenterToVertex { get { return CellSize / Root3; } }
    //    public float CenterToEdge { get { return CellSize * Root3 / 6; } }
    //    public int Resolution { get; set; }
    //    public int Width => Resolution;
    //    public int Height => Resolution;
    //    public float3 Center { get; set; }
    //    public float CellSize { get; set; }

    //    public bool IsFlipTriangle(int x, int y)
    //    {
    //        if ((y % 2 == 0 && x % 2 == 1) ||
    //                   (y % 2 == 1 && x % 2 == 0))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //    public int WorldToIndex(float2 position, bool alwaysInGrid)
    //    {
    //        // For 5x5 (res 5) grid
    //        // Å£Å•Å£Å•Å£
    //        // Å•Å£Å•Å£Å•
    //        // Å£Å•Å£Å•Å£
    //        // Å•Å£Å•Å£Å•
    //        // Å£Å•Å£Å•Å£
    //        float2 diff = position - Center.xz - new float2((Width + 1) * CellSize * 0.5f, CellHeight * Height) / 2f;
    //        float yFloat = diff.y / (CellSize * H);
    //        int y = (int)yFloat;
    //        float xFloat = diff.x / CellSize;
    //        int x = (int)xFloat;
    //        float yDecimal = math.frac(yFloat);
    //        float xDecimal = math.frac(xFloat);
    //        if (xDecimal != 0.5f) // If x does not lie exactly at the middle of the triangle, needs to evaluate for left and right triangles
    //        {
    //            if ((y & 1) == 0) // If even (up/down pattern) row
    //            {
    //                if (xDecimal < 0.5f)
    //                {
    //                    // If C is on the left of AB, subtract index by 1
    //                    float2 A = float2.zero;
    //                    float2 B = new float2(CellSize * 0.5f, CellHeight);
    //                    float2 C = new float2(xDecimal, yDecimal);
    //                    float d = TriangleUtils.HalfPlaneCheck(C, A, B);
    //                    if (d < 0)
    //                    {
    //                        x--;
    //                    }
    //                }
    //                else
    //                {
    //                    // If C is on the right of AB, add index by 1
    //                    float2 A = new float2(CellSize * 0.5f, CellHeight);
    //                    float2 B = new float2(CellSize, 0f);
    //                    float2 C = new float2(xDecimal, yDecimal);
    //                    float d = TriangleUtils.HalfPlaneCheck(C, A, B);
    //                    if (d > 0)
    //                    {
    //                        x++;
    //                    }
    //                }
    //            }
    //            else // If odd (down/up pattern) row
    //            {
    //                if (xDecimal < 0.5f)
    //                {
    //                    // If C is on the left of AB, subtract index by 1
    //                    float2 A = new float2(0f, CellHeight);
    //                    float2 B = new float2(CellSize * 0.5f, 0f);
    //                    float2 C = new float2(xDecimal, yDecimal);
    //                    float d = TriangleUtils.HalfPlaneCheck(C, A, B);
    //                    if (d < 0)
    //                    {
    //                        x--;
    //                    }
    //                }
    //                else
    //                {
    //                    // If C is on the right of AB, add index by 1
    //                    float2 A = new float2(CellSize * 0.5f, 0f);
    //                    float2 B = new float2(CellSize, CellHeight);
    //                    float2 C = new float2(xDecimal, yDecimal);
    //                    float d = TriangleUtils.HalfPlaneCheck(C, A, B);
    //                    if (d > 0)
    //                    {
    //                        x++;
    //                    }
    //                }
    //            }
    //        }

    //        if (alwaysInGrid)
    //        {
    //            x = math.clamp(x, 0, Width - 1);
    //            y = math.clamp(y, 0, Height - 1);
    //            return y * Width + x;
    //        }

    //        if (x < 0 || x > Width - 1 ||
    //            y < 0 || y > Height - 1)
    //        {
    //            return -1;
    //        }
    //        return y * Width + x;
    //    }

    //    public int GridToIndex(int2 xy)
    //    {
    //        return xy.y * Width + xy.x;
    //    }
    //}
}

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
    }
}

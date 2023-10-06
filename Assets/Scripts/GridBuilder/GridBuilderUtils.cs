using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

using BufferElement = StrengthInNumber.GridBuilder.GridBuilder_GridBufferElement;
using BufferSettings = StrengthInNumber.GridBuilder.GridBuilder_GridBufferSettings;

namespace StrengthInNumber.GridBuilder
{
    public static class GridBuilderUtils
    {
        public static void GetQuery(EntityManager em, out EntityQuery query)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GridBuilder_MainTag>();
            query = builder.Build(em);
            builder.Dispose();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBufferLookup(ref SystemState state, out BufferLookup<BufferElement> lookup, bool isReadOnly)
        {
            lookup = state.GetBufferLookup<BufferElement>(isReadOnly);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBuffer(EntityManager em, Entity entity, out DynamicBuffer<BufferElement> buffer, bool isReadOnly)
        {
            buffer = em.GetBuffer<BufferElement>(entity, isReadOnly);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBufferSettings(EntityManager em, Entity entity, out BufferSettings settings)
        {
            settings = em.GetSharedComponent<BufferSettings>(entity);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 WorldToGrid(float2 input, float2 center, int2 gridSize, float2 cellSize, bool alwaysInGrid)
        {
            int2 nullResult = new int2(-1);
            float2 bottomLeft = center - cellSize * gridSize / 2f;
            float2 diff = input - bottomLeft;
            if(alwaysInGrid)
            {
                int x = math.clamp((int)(diff.x / cellSize.x), 0, gridSize.x - 1);
                int y = math.clamp((int)(diff.y / cellSize.y), 0, gridSize.y - 1);
                return new int2(x, y);
            }
            else
            {
                if(diff.x * diff.y < 0)
                {
                    return nullResult;
                }

                int x = (int)(diff.x / cellSize.x);
                int y = (int)(diff.y / cellSize.y);

                if(x >= gridSize.x ||
                    y >= gridSize.y)
                {
                    return nullResult;
                }
                return new int2(x, y);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GridToIndex(int2 input, int gridWidth)
        {
            return input.y * gridWidth + input.x;
        }
    }
}

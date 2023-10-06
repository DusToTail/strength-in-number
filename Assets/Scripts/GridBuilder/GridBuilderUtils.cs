using Unity.Entities;
using Unity.Collections;
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
    }
}

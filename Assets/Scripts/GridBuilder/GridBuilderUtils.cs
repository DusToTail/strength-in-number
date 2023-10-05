using Unity.Entities;
using Unity.Collections;

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
    }
}

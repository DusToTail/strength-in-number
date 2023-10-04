using Unity.Entities;
using Unity.Collections;

namespace StrengthInNumber.GridBuilder
{
    public static class GridBuilderUtils
    {
        public static void GetBuilderQuery(EntityManager em, out EntityQuery query)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Input_GridBuilder_Tag>();
            // There will be more components here
            query = builder.Build(em);
            builder.Dispose();
        }
    }
}

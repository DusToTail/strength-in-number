using Unity.Entities;
using Unity.Collections;

namespace StrengthInNumber
{
    public static class InputUtils
    {
        public static void GetInputQuery(EntityManager em, out EntityQuery query)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Input_Mouse_Position, Input_Mouse_Select, Input_Mouse_Deselect>()
                .WithAll<Input_Keyboard_Left, Input_Keyboard_Right, Input_Keyboard_Up, Input_Keyboard_Down, Input_Keyboard_Confirm, Input_Keyboard_Cancel>()
                .WithDisabled<Input_Disabled_Flag>();

            query = builder.Build(em);
            builder.Dispose();
        }
    }
}

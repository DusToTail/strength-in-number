using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber
{
    public class MovementAuthoring : MonoBehaviour
    {
        public EntityType type;
        public EntityShapeType shapeType;
        public int sizeTier;

        public class MovementBaker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
            }
        }
    }

    public enum Movement_Type
    {
        Cube,
        Pyramid,
        Sphere
    }
    public struct Movement_Cube : IComponentData
    {
    }
    public struct Movement_Pyramid : IComponentData
    {
    }
    public struct Movement_Sphere : IComponentData
    {
    }
    public struct Movement_EnabledFlag : IComponentData, IEnableableComponent
    {
    }
    public struct Movement_BeforeMovementFlag : IComponentData, IEnableableComponent
    {
    }
    public struct Movement_DuringMovementFlag : IComponentData, IEnableableComponent
    {
    }
    public struct Movement_AfterMovementFlag : IComponentData, IEnableableComponent
    {
    }
    public struct Movement_State : IComponentData
    {
    }
}

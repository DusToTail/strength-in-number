using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber
{
    public class EntityAuthoring : MonoBehaviour
    {
        public EntityType type;
        public EntityShapeType shapeType;
        public int sizeTier;

        public class EntityBaker : Baker<EntityAuthoring>
        {
            public override void Bake(EntityAuthoring authoring)
            {
            }
        }
    }

    public enum EntityType
    {
        Unit,
        Building
    }
    public enum EntityShapeType
    {
        Cube,
        Pyramid,
        Sphere
    }

    public struct Entity_UnitTag : IComponentData
    {
    }
    public struct Entity_BuildingTag : IComponentData
    {
    }
    public struct Entity_CubeShapeTag : IComponentData
    {
    }
    public struct Entity_PyramidShapeTag : IComponentData
    {
    }
    public struct Entity_ShpereShapeTag : IComponentData
    {
    }
    public struct Entity_GameplayInfo : IComponentData
    {
        public int sizeTier;
        public Bounds bounds;
        public float3 position;
    }
}

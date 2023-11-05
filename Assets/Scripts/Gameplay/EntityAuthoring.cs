using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber
{
    public enum EntityType
    {
        Unit,
        Building
    }
    public enum EntityShapeType
    {
        Cube,
        Tetrahedron
    }
    public partial class EntityAuthoring : MonoBehaviour
    {
        public EntityType type;
        public EntityShapeType shape;
        public int sizeTier;

        public class EntityBaker : Baker<EntityAuthoring>
        {
            private Entity _self;
            public override void Bake(EntityAuthoring authoring)
            {
                _self = GetEntity(TransformUsageFlags.Dynamic |
                                    TransformUsageFlags.Renderable |
                                    TransformUsageFlags.WorldSpace);

                AddEntityType(authoring.type);
                AddShape(authoring.shape);
                AddGameplay(authoring.sizeTier, authoring.transform.position);
            }

            private void AddEntityType(EntityType type)
            {
                switch (type)
                {
                    case EntityType.Unit:
                        {
                            AddComponent(_self, new Entity_UnitTag());
                            break;
                        }
                    case EntityType.Building:
                        {
                            AddComponent(_self, new Entity_BuildingTag());
                            break;
                        }
                }
            }

            private void AddShape(EntityShapeType shape)
            {
                switch (shape)
                {
                    case EntityShapeType.Cube:
                        {
                            AddComponent(_self, new Entity_CubeTag());
                            break;
                        }
                    case EntityShapeType.Tetrahedron:
                        {
                            AddComponent(_self, new Entity_TetrahedronTag());
                            break;
                        }
                }
            }

            private void AddGameplay(int sizeTier, float3 position)
            {
                AddComponent(_self, new Entity_GameplayInfo
                {
                    sizeTier = sizeTier,
                    position = position
                });
            }
        }
    }

    public struct Entity_UnitTag : IComponentData
    {
    }
    public struct Entity_BuildingTag : IComponentData
    {
    }
    public struct Entity_CubeTag : IComponentData
    {
    }
    public struct Entity_TetrahedronTag : IComponentData
    {
    }
    public struct Entity_GameplayInfo : IComponentData
    {
        public int sizeTier;
        public float3 position;
    }
}

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber
{
    public class EntityAuthoring : MonoBehaviour
    {
        public Grid2DAuthoring grid2DAuthoring;
        public EntityType type;
        public EntityShapeType shapeType;
        public int sizeTier;

#if UNITY_EDITOR
        [SerializeField]
        private Mesh drawMesh;
        [SerializeField]
        private bool drawGizmos;

        public const float Root3 = 1.732f;
        public const float Root6 = 2.449f;

        private void OnValidate()
        {
            switch (shapeType)
            {
                case EntityShapeType.Cube:
                    {
                        drawMesh = new Mesh();
                        drawMesh.vertices = new Vector3[]
                        {
                            new Vector3(-0.5f, 0f, -0.5f),
                            new Vector3(0.5f, 0f, -0.5f),
                            new Vector3(0.5f, 0f, 0.5f),
                            new Vector3(-0.5f, 0f, 0.5f),
                            new Vector3(-0.5f, 1f, -0.5f),
                            new Vector3(0.5f, 1f, -0.5f),
                            new Vector3(0.5f, 1f, 0.5f),
                            new Vector3(-0.5f, 1f, 0.5f)
                        };
                        drawMesh.triangles = new int[]
                        {
                            0,3,1,
                            1,3,2,
                            0,1,4,
                            1,5,4,
                            1,2,5,
                            2,6,5,
                            2,3,6,
                            3,7,6,
                            3,0,7,
                            0,4,7,
                            4,5,7,
                            5,6,7
                        };
                        drawMesh.RecalculateBounds();
                        drawMesh.RecalculateNormals(UnityEngine.Rendering.MeshUpdateFlags.DontRecalculateBounds);
                        break;
                    }
                case EntityShapeType.Pyramid:
                    {
                        drawMesh = new Mesh();
                        drawMesh.vertices = new Vector3[]
                        {
                            new Vector3(-0.5f, 0f, -Root3 / 6f),
                            new Vector3(0.5f, 0f, -Root3 / 6f),
                            new Vector3(0f, 0f, 1f / Root3),
                            new Vector3(0f, Root6 / 3f, 0f)
                        };
                        drawMesh.triangles = new int[]
                        {
                            0,2,1,
                            0,1,3,
                            1,2,3,
                            3,0,1
                        };
                        drawMesh.RecalculateBounds();
                        drawMesh.RecalculateNormals(UnityEngine.Rendering.MeshUpdateFlags.DontRecalculateBounds);
                        break;
                    }
                case EntityShapeType.Sphere:
                    {
                        drawMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                        break;
                    }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) { return; }
            if (drawMesh == null)
            {
                Debug.LogWarning("Draw Mesh is null");
                return;
            }
            Gizmos.color = Color.yellow;
            Vector3 position = transform.position;
            switch (shapeType)
            {
                case EntityShapeType.Cube:
                    {
                        break;
                    }
                case EntityShapeType.Pyramid:
                    {
                        break;
                    }
                case EntityShapeType.Sphere:
                    {
                        break;
                    }
            }

            Gizmos.DrawWireMesh(drawMesh, position, Quaternion.identity);
        }
#endif
        public class EntityBaker : Baker<EntityAuthoring>
        {
            public override void Bake(EntityAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.Dynamic |
                                    TransformUsageFlags.Renderable |
                                    TransformUsageFlags.WorldSpace);
                var grid = authoring.grid2DAuthoring;
                float cellSize = grid.cellSize;
                int sizeTier = authoring.sizeTier;
                Bounds bounds = new Bounds();
                switch(authoring.type)
                {
                    case EntityType.Unit:
                        {
                            AddComponent(self, new Entity_UnitTag());
                            break;
                        }
                    case EntityType.Building:
                        {
                            AddComponent(self, new Entity_BuildingTag());
                            break;
                        }
                }

                switch (authoring.shapeType)
                {
                    case EntityShapeType.Cube:
                        {
                            bounds = new Bounds(Vector3.one * 0.5f * cellSize * sizeTier, Vector3.one * cellSize * sizeTier);
                            AddComponent(self, new Entity_CubeShapeTag());
                            break;
                        }
                    case EntityShapeType.Pyramid:
                        {
                            float root3 = 1.732f;
                            float root6 = 2.449f;
                            float3 offset = new float3(0.5f, root6 / 12f, root3 / 6f) * cellSize;
                            bounds = new Bounds(offset * sizeTier, new Vector3(1f, root6 / 3f, 1f) * sizeTier);
                            AddComponent(self, new Entity_PyramidShapeTag());
                            break;
                        }
                    case EntityShapeType.Sphere:
                        {
                            bounds = new Bounds(Vector3.one * 0.5f * cellSize * sizeTier, Vector3.one * cellSize * sizeTier);
                            AddComponent(self, new Entity_SphereShapeTag());
                            break;
                        }
                }

                AddComponent(self, new Entity_GameplayInfo
                {
                    sizeTier = sizeTier,
                    bounds = bounds,
                    position = authoring.transform.position
                });
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
    public struct Entity_SphereShapeTag : IComponentData
    {
    }
    public struct Entity_GameplayInfo : IComponentData
    {
        public int sizeTier;
        public Bounds bounds;
        public float3 position;
    }
}

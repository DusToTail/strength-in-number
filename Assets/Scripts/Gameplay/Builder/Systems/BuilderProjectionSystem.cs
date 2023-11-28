using Unity.Entities;
using Unity.Mathematics;
using StrengthInNumber.Grid;
using UnityEngine;
using Unity.Rendering;

namespace StrengthInNumber.Builder
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(EntitiesGraphicsSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial class BuilderProjectionSystem : SystemBase
    {
        private Mesh _squareGridMesh;
        private Mesh _triangleGridMesh;
        private Material _gridMaterial;
        private Matrix4x4[] _gridMatrices;
        private Material _projectionMaterial;
        private int _projectionLayer = 1;

        protected override void OnCreate()
        {
            _squareGridMesh = new Mesh();
            _squareGridMesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(-0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, 0.5f),
            };
            _squareGridMesh.normals = new Vector3[]{
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
            };
            _squareGridMesh.triangles = new int[]
            {
                0,2,1,
                2,3,1
            };

            _triangleGridMesh = new Mesh();
            _triangleGridMesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0f, -math.sqrt(3) / 6),
                new Vector3(0.5f, 0f, -math.sqrt(3) / 6),
                new Vector3(0f, 0f, 1 / math.sqrt(3)),
            };
            _triangleGridMesh.normals = new Vector3[]{
                Vector3.up,
                Vector3.up,
                Vector3.up,
            };
            _triangleGridMesh.triangles = new int[]
            {
                 0,2,1,
            };

            _gridMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            _gridMaterial.enableInstancing = true;
            _projectionMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        protected override void OnUpdate()
        {
            var prefabs = SystemAPI.GetSingletonBuffer<BuildPrefab>(true);
            var builder = SystemAPI.GetSingletonRW<Builder>();

            var gridPosition = builder.ValueRO.gridPosition;
            var prefabIndex = builder.ValueRO.prefabIndex;
            var faceEnum = builder.ValueRO.faceEnum;
            var gridType = builder.ValueRO.gridType;

            Entity prefab = prefabs[prefabIndex].prefab;
            var materialMeshInfo = EntityManager.GetComponentData<MaterialMeshInfo>(prefab);
            var renderMeshArray = EntityManager.GetSharedComponentManaged<RenderMeshArray>(prefab);
            Mesh mesh = renderMeshArray.GetMesh(materialMeshInfo);

            float3 position = default;
            quaternion rotation = quaternion.identity;
            if (gridType == Builder.GridType.Square)
            {
                var grid = ((RefRO<SquareGrid>)SystemAPI.GetSingletonRW<SquareGrid>());
                DrawSquareGrid(grid);

                position = grid.ValueRO.GridToWorld(gridPosition.x, gridPosition.y);
                position.y += grid.ValueRO.CellSize / 2f;

                if (faceEnum != 0)
                {
                    var face = (SquareGridUtils.Faces)faceEnum;
                    var dirInt2 = SquareGridUtils.ToInt2(face);
                    float3 dir = new float3(dirInt2.x, 0f, dirInt2.y);
                    rotation = quaternion.LookRotation(dir, new float3(0f, 1f, 0f));
                }
            }
            else if(gridType == Builder.GridType.Triangle)
            {
                var grid = ((RefRO<TriangleGrid>)SystemAPI.GetSingletonRW<TriangleGrid>());
                DrawTriangleGrid(grid);

                position = grid.ValueRO.GridToWorld(gridPosition.x, gridPosition.y);
                position.y += grid.ValueRO.CellSize * math.sqrt(6) / 12f;

                //if (faceEnum != 0)
                //{
                //    var face = (SquareGridUtils.Faces)faceEnum;
                //    var dirInt2 = SquareGridUtils.ToInt2(face);
                //    float3 dir = new float3(dirInt2.x, 0f, dirInt2.y);
                //    rotation = quaternion.LookRotation(dir, new float3(0f, 1f, 0f));
                //}
            }
            DrawProjection(position, rotation, mesh);
        }

        private void DrawSquareGrid(RefRO<SquareGrid> grid)
        {
            int count = grid.ValueRO.Height * grid.ValueRO.Width;
            float size = grid.ValueRO.CellSize;
            if(_gridMatrices == null || _gridMatrices.Length != count)
            {
                _gridMatrices = new Matrix4x4[count];
            }
            for (int y = 0; y < grid.ValueRO.Height; y++)
            {
                for (int x = 0; x < grid.ValueRO.Width; x++)
                {
                    _gridMatrices[y * grid.ValueRO.Width + x] = Matrix4x4.TRS(grid.ValueRO.GridToWorld(x, y), Quaternion.identity, Vector3.one * size);
                }
            }
            Graphics.DrawMeshInstanced(_squareGridMesh, 0, _gridMaterial, _gridMatrices, count);
        }

        private void DrawTriangleGrid(RefRO<TriangleGrid> grid)
        {
            int count = grid.ValueRO.Height * grid.ValueRO.Width;
            float size = grid.ValueRO.CellSize;
            if (_gridMatrices == null || _gridMatrices.Length != count)
            {
                _gridMatrices = new Matrix4x4[count];
            }
            for (int y = 0; y < grid.ValueRO.Height; y++)
            {
                for (int x = 0; x < grid.ValueRO.Width; x++)
                {
                    bool flip = TriangleGrid.IsFlipTriangle(x, y);
                    _gridMatrices[y * grid.ValueRO.Width + x] = Matrix4x4.TRS(grid.ValueRO.GridToWorld(x, y), Quaternion.identity, new Vector3(1f, 1f, flip ? -1f : 1f) * size);
                }
            }
            Graphics.DrawMeshInstanced(_triangleGridMesh, 0, _gridMaterial, _gridMatrices, count);
        }

        private void DrawProjection(float3 position, quaternion rotation, Mesh mesh)
        {
            Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), _projectionMaterial, _projectionLayer);
        }
    }
}

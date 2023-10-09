using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber
{
    public enum Shape
    {
        Square,
        Triangle
    }

    public interface IGridSettings
    {
        int Resolution { get; set; }
        int Width { get; }
        int Height { get; }
        float3 Center { get; set; }
        Shape Shape { get; }
        float CellSize { get; set; }

        int WorldToIndex(float2 position, bool alwaysInGrid);
    }

    public class Grid2DAuthoring : MonoBehaviour
    {
        public int resolution;
        public Shape shape;
        public Vector3 center;
        public float cellSize;
        public GameObject cellPrefab;


#if UNITY_EDITOR
        [SerializeField]
        private Vector3[] drawCenters;
        [SerializeField]
        private Mesh drawMesh;
        [SerializeField]
        private bool drawGizmos;

        private void OnValidate()
        {
            switch (shape)
            {
                case Shape.Square:
                    {
                        drawMesh = new Mesh();
                        drawMesh.vertices = new Vector3[]
                        {
                            new Vector3(-0.5f, 0f, -0.5f),
                            new Vector3(0.5f, 0f, -0.5f),
                            new Vector3(-0.5f, 0f, 0.5f),
                            new Vector3(0.5f, 0f, 0.5f)
                        };
                        drawMesh.normals = new Vector3[]{
                            Vector3.up,
                            Vector3.up,
                            Vector3.up,
                            Vector3.up,
                        };
                        drawMesh.triangles = new int[]
                        {
                            0,1,2,
                            1,3,2
                        };
                        SetupSquareGridCenters();
                        break;
                    }
                case Shape.Triangle:
                    {
                        drawMesh = new Mesh();
                        drawMesh.vertices = new Vector3[]
                        {
                            new Vector3(-0.5f, 0f, -CenterToEdge),
                            new Vector3(0.5f, 0f, -CenterToEdge),
                            new Vector3(0f, 0f, CenterToVertex),
                        };
                        drawMesh.normals = new Vector3[]{
                            Vector3.up,
                            Vector3.up,
                            Vector3.up,
                        };
                        drawMesh.triangles = new int[]
                        {
                            0,1,2,
                        };
                        SetupTriangleGridCenters();
                        break;
                    }
            }
        }

        private void SetupSquareGridCenters()
        {
            drawCenters = new Vector3[resolution * resolution];
            float2 offset = new float2(cellSize / 2f) - new float2(resolution * cellSize) / 2f;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float xPosition = x * cellSize + center.x + offset.x;
                    float yPosition = center.y;
                    float zPosition = y * cellSize + center.z + offset.y;
                    int index = y * resolution + x;
                    drawCenters[index] = new Vector3(xPosition, yPosition, zPosition);
                }
            }
        }

        public const float Root3 = 1.732f;
        public const float H = Root3 / 2f;
        public float CellHeight { get { return cellSize * H; } }
        public float CenterToVertex { get { return cellSize / Root3; } }
        public float CenterToEdge { get { return cellSize * Root3 / 6; } }

        private void SetupTriangleGridCenters()
        {
            drawCenters = new Vector3[resolution * resolution * 2];
            float2 offset = new float2(cellSize / 2f, CenterToEdge) - new float2((resolution * 2 + 1) * cellSize * 0.5f, CellHeight * resolution) / 2f;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution * 2; x++)
                {
                    float xPosition = x * cellSize / 2f + center.x + offset.x;
                    float yPosition = center.y;
                    float zPosition = AlternateHeightSum(y, CenterToEdge, CenterToVertex, (x & 1) == 0)
                        + center.z + offset.y;
                    int index = y * resolution * 2 + x;
                    drawCenters[index] = new Vector3(xPosition, yPosition, zPosition);
                }
            }

            // Moved to a function for readability
            float AlternateHeightSum(int y, float centerToEdge, float centerToVertex, bool evenColumn)
            {
                float result = ((y / 2) * 2 + 1) * (evenColumn ? centerToEdge : centerToVertex) +
                               (((y + 1) / 2) * 2) * (evenColumn ? centerToVertex : centerToEdge);
                return result;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) { return; }
            if(drawMesh == null)
            {
                Debug.LogWarning("Draw Mesh is null");
                return;
            }
            Gizmos.color = Color.yellow;
            switch(shape)
            {
                case Shape.Square:
                    {
                        DrawGizmosSquareGrid();
                        break;
                    }
                case Shape.Triangle:
                    {
                        DrawGizmosTriangleGrid();
                        break;
                    }
            }
        }

        private void DrawGizmosSquareGrid()
        {
            for(int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    Vector3 position = drawCenters[x + y * resolution];
                    Gizmos.DrawWireMesh(drawMesh, position, Quaternion.identity, Vector3.one * cellSize);
                    Gizmos.DrawSphere(position, 0.1f);
                }
            }
        }
        private void DrawGizmosTriangleGrid()
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution * 2; x++)
                {
                    Vector3 position = drawCenters[x + y * resolution * 2];
                    bool flip = false;
                    if((y % 2 == 0 && x % 2 == 1) ||
                       (y % 2 == 1 && x % 2 == 0))
                    {
                        flip = true;
                    }
                    Gizmos.DrawWireMesh(drawMesh, position, Quaternion.identity, new Vector3(1f, 1f, flip ? -1f : 1f) * cellSize);
                    Gizmos.DrawSphere(position, 0.1f);
                }
            }
        }
#endif

        public class Grid2DBaker : Baker<Grid2DAuthoring>
        {
            public override void Bake(Grid2DAuthoring authoring)
            {
                switch(authoring.shape)
                {
                    case Shape.Square:
                        {
                            AddSquareGrid(
                                authoring.resolution,
                                authoring.center,
                                authoring.cellSize,
                                authoring.cellPrefab);
                            break;
                        }
                    case Shape.Triangle:
                        {
                            AddTriangleGrid(
                                authoring.resolution,
                                authoring.center,
                                authoring.cellSize,
                                authoring.cellPrefab);
                            break;
                        }
                }
            }

            private void AddSquareGrid(int resolution, float3 center, float cellSize, GameObject prefab)
            {
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                var settings = new SquareGridSettings()
                {
                    CellPrefab = new EntityPrefabReference(prefab),
                    Resolution = resolution,
                    Center = center,
                    CellSize = cellSize,
                };
                AddSharedComponent(self, settings);

                var cells = new NativeArray<GridBufferElementData>(settings.Width * settings.Height, Allocator.Temp);
                float2 offset = new float2(cellSize / 2f) - new float2(settings.Width, settings.Height) * cellSize / 2f;
                for (int y = 0; y < settings.Height; y++)
                {
                    for (int x = 0; x < settings.Width; x++)
                    {
                        float xPosition = x * cellSize + center.x + offset.x;
                        float yPosition = center.y;
                        float zPosition = y * cellSize + center.z + offset.y;
                        int index = y * settings.Width + x;
                        cells[index] = new GridBufferElementData
                        {
                            position = new float3(xPosition, yPosition, zPosition),
                            index = index,
                            entity = Entity.Null
                        };
                    }
                }
                // Still need to instatiate the prefab per element in a system
                var buffer = AddBuffer<GridBufferElementData>(self);
                buffer.EnsureCapacity(cells.Length);
                buffer.AddRange(cells);
                cells.Dispose();
            }

            private void AddTriangleGrid(int resolution, float3 center, float cellSize, GameObject prefab)
            {
                var self = GetEntity(TransformUsageFlags.WorldSpace);

                var settings = new TriangleGridSettings()
                {
                    CellPrefab = new EntityPrefabReference(prefab),
                    Resolution = resolution,
                    Center = center,
                    CellSize = cellSize,
                };
                AddSharedComponent(self, settings);

                var cells = new NativeArray<GridBufferElementData>(settings.Width * settings.Height, Allocator.Temp);
                float2 offset = new float2(cellSize / 2f, settings.CenterToEdge) - 
                    new float2((settings.Width + 1) * cellSize * 0.5f, settings.CellHeight * settings.Height) / 2f;
                for (int y = 0; y < settings.Height; y++)
                {
                    for (int x = 0; x < settings.Width; x++)
                    {
                        float xPosition = x * cellSize / 2f + center.x + offset.x;
                        float yPosition = center.y;
                        float zPosition = AlternateHeightSum(y, settings.CenterToEdge, settings.CenterToVertex, (x & 1) == 0) 
                            + center.z + offset.y;
                        int index = y * settings.Width + x;
                        cells[index] = new GridBufferElementData
                        {
                            position = new float3(xPosition, yPosition, zPosition),
                            index = index,
                            entity = Entity.Null
                        };
                    }
                }
                // Still need to instatiate the prefab per element in a system
                var buffer = AddBuffer<GridBufferElementData>(self);
                buffer.EnsureCapacity(cells.Length);
                buffer.AddRange(cells);
                cells.Dispose();

                // Moved to a function for readability
                float AlternateHeightSum(int y, float centerToEdge, float centerToVertex, bool evenColumn)
                {
                    float result = ((y / 2) * 2 + 1) * (evenColumn ? centerToEdge : centerToVertex) +
                                   (((y + 1) / 2) * 2) * (evenColumn ? centerToVertex : centerToEdge);
                    return result;
                }
            }
        }
    }

    public struct GridBufferElementData : IBufferElementData
    {
        public Entity entity;
        public float3 position;
        public int index;
    }

    public struct SquareGridElement : IComponentData
    {
        public int index;
    }
    public struct TriangleGridElement : IComponentData
    {
        public int index;
    }
    public struct SquareGridSettings : ISharedComponentData, IGridSettings
    {
        public EntityPrefabReference CellPrefab;

        public int Resolution { get; set; }
        public int Width => Resolution;
        public int Height => Resolution;
        public float3 Center { get; set; }
        public Shape Shape => Shape.Square;
        public float CellSize { get; set; }

        public int WorldToIndex(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            float2 diff = position - Center.xz - new float2(CellSize * Width, CellSize * Height) / 2;
            int x = (int)(diff.x / CellSize);
            int y = (int)(diff.y / CellSize);
            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, Width - 1);
                y = math.clamp(y, 0, Height - 1);
                return y * Width + x;
            }

            if (x < 0 || x > Width - 1 ||
                y < 0 || y > Height - 1)
            {
                return -1;
            }
            return y * Width + x;
        }
    }
    public struct TriangleGridSettings : ISharedComponentData, IGridSettings
    {
        public EntityPrefabReference CellPrefab;
        public const float Root3 = 1.732f;
        public const float H = Root3 / 2f;
        public float CellHeight { get { return CellSize * H; } }
        public float CenterToVertex { get { return CellSize / Root3; } }
        public float CenterToEdge { get { return CellSize * Root3 / 6; } }
        public int Resolution { get; set; }
        public int Width => Resolution;
        public int Height => Resolution;
        public float3 Center { get; set; }
        public Shape Shape => Shape.Triangle;
        public float CellSize { get; set; }

        public bool IsFlipTriangle(int x, int y)
        {
            if ((y % 2 == 0 && x % 2 == 1) ||
                       (y % 2 == 1 && x % 2 == 0))
            {
                return true;
            }
            return false;
        }
        public int WorldToIndex(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            float2 diff = position - Center.xz - new float2((Width + 1) * CellSize * 0.5f, CellHeight * Height) / 2f;
            float yFloat = diff.y / (CellSize * H);
            int y = (int)yFloat;
            float xFloat = diff.x / CellSize;
            int x = (int)xFloat;
            float yDecimal = math.frac(yFloat);
            float xDecimal = math.frac(xFloat);
            if (xDecimal != 0.5f) // If x does not lie exactly at the middle of the triangle, needs to evaluate for left and right triangles
            {
                if ((y & 1) == 0) // If even (up/down pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = float2.zero;
                        float2 B = new float2(CellSize * 0.5f, CellHeight);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(CellSize * 0.5f, CellHeight);
                        float2 B = new float2(CellSize, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
                else // If odd (down/up pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = new float2(0f, CellHeight);
                        float2 B = new float2(CellSize * 0.5f, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(CellSize * 0.5f, 0f);
                        float2 B = new float2(CellSize, CellHeight);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
            }

            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, Width - 1);
                y = math.clamp(y, 0, Height - 1);
                return y * Width + x;
            }

            if (x < 0 || x > Width - 1 ||
                y < 0 || y > Height - 1)
            {
                return -1;
            }
            return y * Width + x;
        }
    }
}

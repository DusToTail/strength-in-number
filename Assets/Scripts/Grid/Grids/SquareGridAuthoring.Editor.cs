#if UNITY_EDITOR
using UnityEngine;
using Unity.Mathematics;

namespace StrengthInNumber.Grid
{
    public partial class SquareGridAuthoring : GridAuthoring
    {
        protected override Vector3[] DrawCenters => _drawCenters;
        protected override Mesh DrawMesh => _drawMesh;
        protected override bool DrawGizmos => _drawGizmos;

        [Header("EDITOR_ONLY")]
        [SerializeField]
        private Mesh _drawMesh;
        [SerializeField]
        private bool _drawGizmos;
        private Vector3[] _drawCenters;

        public override float3 GridToWorld(int x, int y, float heightOffset = 0)
        {
            float2 offset = new float2(cellSize / 2f) - new float2(width, height) * cellSize / 2f;

            float xPosition = x * cellSize + center.x + offset.x;
            float yPosition = center.y + heightOffset;
            float zPosition = y * cellSize + center.z + offset.y;
            return new float3(xPosition, yPosition, zPosition);
        }
        public Vector2Int IndexToGrid(int index)
        {
            int y = index / width;
            int x = index % width;
            return new Vector2Int(x, y);
        }
        // **********************Copy from SquareGridAuthoring.cs (duplicated)************************
        public int WorldToIndex(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            float2 diff = position - new float2(center.x, center.z) + new float2(cellSize * width, cellSize * height) / 2;
            int x = (int)(diff.x / cellSize);
            int y = (int)(diff.y / cellSize);
            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, width - 1);
                y = math.clamp(y, 0, height - 1);
                return y * width + x;
            }

            if (x < 0 || x > width - 1 ||
                y < 0 || y > height - 1)
            {
                return -1;
            }
            return y * width + x;
        }

        public int GridToIndex(int x, int y)
        {
            return y * width + x;
        }
        // ************************************************************************

        protected override void GizmosDraw()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 position = _drawCenters[x + y * width];
                    Gizmos.DrawWireMesh(_drawMesh, position, Quaternion.identity, Vector3.one * cellSize);
                    Gizmos.DrawSphere(position, 0.1f);
                }
            }
        }

        protected override void GizmosSetup()
        {
            _drawMesh = new Mesh();
            _drawMesh.vertices = new Vector3[]
            {
                            new Vector3(-0.5f, 0f, -0.5f),
                            new Vector3(0.5f, 0f, -0.5f),
                            new Vector3(0.5f, 0f, 0.5f),
                            new Vector3(-0.5f, 0f, 0.5f)
            };
            _drawMesh.normals = new Vector3[]{
                            Vector3.up,
                            Vector3.up,
                            Vector3.up,
                            Vector3.up,
                        };
            _drawMesh.triangles = new int[]
            {
                            0,3,1,
                            3,2,1
            };

            _drawCenters = new Vector3[width * height];
            float2 offset = new float2(cellSize / 2f) - new float2(width, height) * cellSize / 2f;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    _drawCenters[index] = GridToWorld(x, y);
                }
            }
        }
    }
}
#endif

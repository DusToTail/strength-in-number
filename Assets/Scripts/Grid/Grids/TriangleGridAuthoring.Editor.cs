#if UNITY_EDITOR
using UnityEngine;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace StrengthInNumber.Grid
{
    public partial class TriangleGridAuthoring : GridAuthoring
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

        private float CellHeight { get { return cellSize * sqrt(3) / 2f; } }
        private float CenterToVertex { get { return cellSize / sqrt(3); } }
        private float CenterToEdge { get { return cellSize * sqrt(3) / 6; } }

        public override float3 GridToWorld(int x, int y, float heightOffset = 0)
        {
            float2 offset = new float2(cellSize / 2f, CenterToEdge) - new float2((width + 1) * cellSize * 0.5f, CellHeight * height) / 2f;
            float xPosition = x * cellSize / 2f + center.x + offset.x;
            float yPosition = center.y + heightOffset;
            float zPosition = AlternateHeightSum(y, CenterToEdge, CenterToVertex, (x & 1) == 0)
                + center.z + offset.y;
            return new Vector3(xPosition, yPosition, zPosition);
        }

        // **********************Copy from TriangleGridAuthoring.cs (duplicated)************************
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
            float2 diff = position - new float2(center.x, center.z) + new float2((width + 1) * cellSize * 0.5f, CellHeight * height) / 2f;
            float yFloat = diff.y / CellHeight;
            int y = (int)yFloat;
            float xFloat = diff.x / cellSize;
            int x = (int)xFloat;
            float yDecimal = frac(yFloat);
            float xDecimal = frac(xFloat);
            if (xDecimal != 0.5f) // If x does not lie exactly at the middle of the triangle, needs to evaluate for left and right triangles
            {
                if ((y & 1) == 0) // If even (up/down pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = float2(0);
                        float2 B = new float2(cellSize * 0.5f, CellHeight);
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
                        float2 A = new float2(cellSize * 0.5f, CellHeight);
                        float2 B = new float2(cellSize, 0f);
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
                        float2 B = new float2(cellSize * 0.5f, 0f);
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
                        float2 A = new float2(cellSize * 0.5f, 0f);
                        float2 B = new float2(cellSize, CellHeight);
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
                x = clamp(x, 0, width - 1);
                y = clamp(y, 0, height - 1);
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
                    bool flip = false;
                    if ((y % 2 == 0 && x % 2 == 1) ||
                       (y % 2 == 1 && x % 2 == 0))
                    {
                        flip = true;
                    }
                    Gizmos.DrawWireMesh(_drawMesh, position, Quaternion.identity, new Vector3(1f, 1f, flip ? -1f : 1f) * cellSize);
                    Gizmos.DrawSphere(position, 0.1f);
                }
            }
        }

        protected override void GizmosSetup()
        {
            _drawMesh = new Mesh();
            _drawMesh.vertices = new Vector3[]
            {
                            new Vector3(-0.5f, 0f, -CenterToEdge),
                            new Vector3(0.5f, 0f, -CenterToEdge),
                            new Vector3(0f, 0f, CenterToVertex),
            };
            _drawMesh.normals = new Vector3[]{
                            Vector3.up,
                            Vector3.up,
                            Vector3.up,
                        };
            _drawMesh.triangles = new int[]
            {
                            0,1,2,
            };

            _drawCenters = new Vector3[width * height];
            float2 offset = new float2(cellSize / 2f, CenterToEdge) - new float2((width + 1) * cellSize * 0.5f, CellHeight * height) / 2f;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    _drawCenters[index] = GridToWorld(x, y);
                }
            }
        }

        // Moved to a function for readability
        private float AlternateHeightSum(int y, float centerToEdge, float centerToVertex, bool evenColumn)
        {
            float result = ((y / 2) * 2 + 1) * (evenColumn ? centerToEdge : centerToVertex) +
                           (((y + 1) / 2) * 2) * (evenColumn ? centerToVertex : centerToEdge);
            return result;
        }
    }
}
#endif

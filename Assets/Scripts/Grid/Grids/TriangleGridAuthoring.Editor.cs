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

        protected override void GizmosDraw()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 position = _drawCenters[x + y * width];
                    bool flip = TriangleGrid.IsFlipTriangle(x, y);
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
                    int index = TriangleGrid.GridToIndex(x, y, width);
                    float2 pos = TriangleGrid.GridToWorld(x, y, new float2(center.x, center.z), cellSize, width, height);
                    _drawCenters[index] = new Vector3(pos.x, center.y, pos.y);
                }
            }
        }
    }
}
#endif

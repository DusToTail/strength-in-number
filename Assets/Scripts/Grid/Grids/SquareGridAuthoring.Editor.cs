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
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = SquareGrid.GridToIndex(x, y, width);
                    float2 pos = SquareGrid.GridToWorld(x, y, cellSize, new float2(center.x, center.z), width, height);
                    _drawCenters[index] = new Vector3(pos.x, center.y, pos.y);
                }
            }
        }
    }
}
#endif

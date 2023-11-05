#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace StrengthInNumber
{
    public partial class EntityAuthoring : MonoBehaviour
    {
        [Header("EDITOR_ONLY")]
        [SerializeField]
        private Mesh drawMesh;
        [SerializeField]
        private bool drawGizmos;

        private void OnValidate()
        {
            string path = "Assets/" + ProceduralMeshes.ProceduralMeshes.RelativePath;
            switch (shape)
            {
                case EntityShapeType.Cube:
                    {
                        drawMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path + "Cube.asset");
                        break;
                    }
                case EntityShapeType.Tetrahedron:
                    {
                        drawMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path + "Tetrahedron.asset");
                        break;
                    }
                default:
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
            switch (shape)
            {
                case EntityShapeType.Cube:
                    {
                        break;
                    }
                case EntityShapeType.Tetrahedron:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            Gizmos.DrawWireMesh(drawMesh, position, Quaternion.identity);
        }
    }
}
#endif

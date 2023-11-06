#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace StrengthInNumber
{
    public partial class EntityAuthoring : MonoBehaviour
    {
        [Header("EDITOR_ONLY")]
        [SerializeField]
        private Mesh usedMesh;
        [SerializeField]
        private bool drawGizmos;

        private void OnValidate() => EditorApplication.update += _OnValidate;
        /// <summary>
        /// To prevent unnecessary warnings about SendMessage in Editor Console. More info below
        /// https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
        /// </summary>
        private void _OnValidate()
        {
            EditorApplication.update -= _OnValidate;
            if (this == null || !EditorUtility.IsDirty(this)) return;

            string path = "Assets/" + ProceduralMeshes.ProceduralMeshes.RelativePath;
            switch (shape)
            {
                case EntityShapeType.Cube:
                    {
                        usedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path + "Cube.asset");
                        break;
                    }
                case EntityShapeType.Tetrahedron:
                    {
                        usedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path + "Tetrahedron.asset");
                        break;
                    }
                default:
                    {
                        usedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                        break;
                    }
            }
            SetupMeshUsage(usedMesh);
        }

        private void SetupMeshUsage(Mesh mesh)
        {
            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter != null)
                filter.sharedMesh = mesh;

            var col = GetComponent<MeshCollider>();
            if (col != null)
                col.sharedMesh = mesh;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) { return; }
            if (usedMesh == null)
            {
                Debug.LogWarning("Mesh missing for DrawGizmos", this);
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

            Gizmos.DrawWireMesh(usedMesh, position, Quaternion.identity);
        }
    }
}
#endif

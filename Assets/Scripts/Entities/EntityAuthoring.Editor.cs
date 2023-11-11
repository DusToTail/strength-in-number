#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace StrengthInNumber.Entities
{
    public abstract partial class EntityAuthoring : MonoBehaviour
    {
        protected abstract string MeshPath { get; }
        protected abstract bool GizmosDraw { get; }
        protected Vector3 simulatedPosition = Vector3.one;
        protected Quaternion simulatedRotation = Quaternion.identity;
        protected Vector3 simulatedScale = Vector3.one;
        private void OnValidate() => EditorApplication.update += _OnValidate;
        /// <summary>
        /// To prevent unnecessary warnings about SendMessage in Editor Console. More info below
        /// https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
        /// </summary>
        private void _OnValidate()
        {
            EditorApplication.update -= _OnValidate;
            if (this == null || !EditorUtility.IsDirty(this)) return;

            mesh = AssetDatabase.LoadAssetAtPath<Mesh>(MeshPath);
            SetupMeshOnValidate(mesh);
            SetupAdditionalOnValidate();
        }

        protected virtual void SetupMeshOnValidate(Mesh mesh)
        {
            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter != null)
                filter.sharedMesh = mesh;

            var col = GetComponent<MeshCollider>();
            if (col != null)
                col.sharedMesh = mesh;
        }
        protected virtual void SetupAdditionalOnValidate() { }

        private void OnDrawGizmos()
        {
            if (!GizmosDraw) { return; }

            simulatedPosition = SimulatePosition();
            simulatedRotation = SimulateRotation();
            simulatedScale = SimulateScale();

            if (mesh == null)
            {
                Debug.LogWarning($"Mesh missing for DrawGizmos ({gameObject})", this);
                return;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireMesh(mesh, simulatedPosition, simulatedRotation, simulatedScale);
        }
        protected abstract Vector3 SimulatePosition();
        protected abstract Quaternion SimulateRotation();
        protected abstract Vector3 SimulateScale();
    }
}
#endif

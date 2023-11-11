#if UNITY_EDITOR
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber.Grid
{
    public abstract partial class GridAuthoring : MonoBehaviour
    {
        abstract protected Vector3[] DrawCenters { get; }
        abstract protected Mesh DrawMesh { get; }
        abstract protected bool DrawGizmos { get; }

        private void OnValidate()
        {
            GizmosSetup();
        }
        private void OnDrawGizmos()
        {
            if (!DrawGizmos) { return; }
            if (DrawMesh == null)
            {
                Debug.LogWarning("Draw Mesh is null");
                return;
            }
            Gizmos.color = Color.yellow;
            GizmosDraw();
        }
        protected abstract void GizmosSetup();
        protected abstract void GizmosDraw();
    }
}
#endif

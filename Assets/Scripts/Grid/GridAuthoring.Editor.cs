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
        private void OnDrawGizmosSelected()
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
        public abstract float3 GridToWorld(int x, int y, float heightOffset = 0f);

        protected abstract void GizmosSetup();
        protected abstract void GizmosDraw();
    }
}
#endif

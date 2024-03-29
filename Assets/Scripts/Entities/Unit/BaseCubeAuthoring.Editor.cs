#if UNITY_EDITOR
using UnityEngine;
using Unity.Mathematics;
using StrengthInNumber.Grid;
namespace StrengthInNumber.Entities
{
    public partial class BaseCubeAuthoring : EntityAuthoring
    {
        protected override string MeshPath => $"Assets/{ProceduralMeshes.ProceduralMeshes.RelativePath}Cube.asset";
        protected override bool GizmosDraw => drawGizmos;

        [Header("EDITOR_ONLY")]
        [SerializeField]
        private bool drawGizmos;
        [SerializeField]
        private SquareGridUtils.Faces direction;
        [SerializeField]
        [Range(0f, 1f)]
        private float lerp;

        protected override void SetupAdditionalOnValidate()
        {
            if(squareGrid == null)
            {
                squareGrid = FindAnyObjectByType<SquareGridAuthoring>(FindObjectsInactive.Exclude);
                Debug.LogWarning($"Automatically set square grid to an active instance in the scene ({gameObject})", this);
            }
            var from = SquareGrid.GridToWorld(position.x, position.y, squareGrid.cellSize, squareGrid.center, squareGrid.width, squareGrid.height);
            from.y += squareGrid.cellSize / 2;
            transform.position = from;

            var rot = Quaternion.identity;
            if (forward != SquareGridUtils.Faces.None)
            {
                var forwardVec = SquareGridUtils.ToInt2(forward);
                rot = Quaternion.LookRotation(new Vector3(forwardVec.x, 0f, forwardVec.y), Vector3.up);
            }
            transform.rotation = rot;
        }

        protected override Vector3 SimulatePosition()
        {
            if (squareGrid == null)
            {
                Debug.LogWarning($"Square Grid reference is null ({gameObject})", this);
                return Vector3.negativeInfinity;
            }
            float3 position = transform.position;
            float3 center = squareGrid.center;
            int2 from = SquareGrid.WorldToGrid(position, center, squareGrid.cellSize, squareGrid.width, squareGrid.height, true);

            Vector2Int dir = SquareGridUtils.ToVector2Int(direction);
            int2 to = from + new int2(dir.x, dir.y);
            if(SquareGridUtils.IsOutOfBound(to.x, to.y, squareGrid.width, squareGrid.height) && lerp > 0)
            {
                Debug.LogWarning($"Cant move from {from} to {to} due to out of bound", this);
                return simulatedPosition;
            }
            float heightOffset = (size / math.SQRT2) * math.sin(math.radians(90f * (lerp + 0.5f)));
            float height = squareGrid.center.y + heightOffset;

            position = SquareGrid.GridToWorld(from.x, from.y, squareGrid.cellSize, center, squareGrid.width, squareGrid.height)
                + new float3(dir.x, 0f, dir.y) * squareGrid.cellSize * lerp;

            this.position = new Vector2Int(from.x, from.y);
            return new Vector3(position.x, height, position.z);
        }

        protected override Quaternion SimulateRotation()
        {
            Vector2 dir = SquareGridUtils.ToVector2Int(direction);
            return transform.rotation * Quaternion.AngleAxis(90f * lerp, math.cross(Vector3.up, new Vector3(dir.x, 0f, dir.y)));
        }

        protected override Vector3 SimulateScale()
        {
            return transform.lossyScale * size;
        }
    }
}
#endif

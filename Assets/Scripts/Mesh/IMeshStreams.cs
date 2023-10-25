using UnityEngine;
using Unity.Mathematics;

namespace StrengthInNumber.ProceduralMeshes
{
    public interface IMeshStreams
    {
        void Setup(Mesh.MeshData data, Bounds bounds, int vertexCount, int indexCount);
        void SetVertex(int index, Vertex data);
        void SetTriangle(int index, int3 triangle);
    }
}

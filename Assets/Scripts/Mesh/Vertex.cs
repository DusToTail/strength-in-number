using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace StrengthInNumber.ProceduralMeshes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public float3 position, normal;
        public float4 tangent;
    }
}

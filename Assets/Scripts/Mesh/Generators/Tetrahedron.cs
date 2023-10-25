using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

using static Unity.Mathematics.math;

namespace StrengthInNumber.ProceduralMeshes
{
    public struct Tetrahedron : IMeshGenerator
    {
        //public static readonly float Height = sqrt(6) / 3;
        //public static readonly float CenterToVertex = Height * 3 / 4;
        //public static readonly float CenterToFace = Height / 4;
        //public static readonly float Height2D = sqrt(3) / 2;
        //public static readonly float Center2DToEdge = Height2D / 3;
        //public static readonly float Center2DToVertex = Height2D * 2 / 3;

        public Bounds Bounds => new Bounds(
            new Vector3(0f, sqrt(6) / 12, sqrt(3) / 6),
            new Vector3(1f, sqrt(6) / 3, sqrt(3) / 2));

        public int VertexCount => 4;

        public int IndexCount => 12;

        public int JobLength => 1;

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            // this is only executed only once, thus will handle all vertices at once
            var positions = new NativeArray<float3>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            positions[0] = new float3(-0.5f, -sqrt(6) / 12, -sqrt(3) / 6);
            positions[1] = new float3( 0.5f, -sqrt(6) / 12, -sqrt(3) / 6);
            positions[2] = new float3( 0.0f, -sqrt(6) / 12,  sqrt(3) / 3);
            positions[3] = new float3( 0.0f,  sqrt(6) / 4 ,  0.0f);
            var tangents = new NativeArray<float4>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            tangents[0] = new float4(1.0f, 0.0f, 0.0f, -1.0f);
            tangents[1] = normalize(new float4(-0.5f, 0.0f,  sqrt(3) / 2, -1.0f));
            tangents[2] = normalize(new float4(-0.5f, 0.0f, -sqrt(3) / 2, -1.0f));
            tangents[3] = new float4(0.0f, 1.0f, 0.0f, -1.0f);

            Vertex vertex = new Vertex();

            for (int vi = 0; vi < 4; vi++)
            {
                vertex.position = positions[vi];
                vertex.normal = normalize(positions[vi]);
                vertex.tangent = tangents[vi];
                streams.SetVertex(vi, vertex);
            }
            streams.SetTriangle(0, new int3(0, 1, 2));
            streams.SetTriangle(1, new int3(0, 3, 1));
            streams.SetTriangle(2, new int3(1, 3, 2));
            streams.SetTriangle(3, new int3(2, 3, 0));

            positions.Dispose();
            tangents.Dispose();
        }
    }
}

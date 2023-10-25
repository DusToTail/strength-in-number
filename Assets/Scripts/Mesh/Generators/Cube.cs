using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

using static Unity.Mathematics.math;

namespace StrengthInNumber.ProceduralMeshes
{
    public struct Cube : IMeshGenerator
    {
        public Bounds Bounds => new Bounds(Vector3.zero, Vector3.one);

        public int VertexCount => 8;

        public int IndexCount => 36;

        public int JobLength => 1;

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            // this is only executed only once, thus will handle all vertices at once
            var positions = new NativeArray<float3>(8, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            positions[0] = new float3( -0.5f,  -0.5f,  -0.5f);
            positions[1] = new float3(  0.5f,  -0.5f,  -0.5f);
            positions[2] = new float3(  0.5f,  -0.5f,   0.5f);
            positions[3] = new float3( -0.5f,  -0.5f,   0.5f);
            positions[4] = new float3( -0.5f,   0.5f,  -0.5f);
            positions[5] = new float3(  0.5f,   0.5f,  -0.5f);
            positions[6] = new float3(  0.5f,   0.5f,   0.5f);
            positions[7] = new float3( -0.5f,   0.5f,   0.5f);
            var tangents = new NativeArray<float4>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            tangents[0] = new float4(   1.0f, 0.0f,  0.0f, -1.0f);
            tangents[1] = new float4(   0.0f, 0.0f,  1.0f, -1.0f);
            tangents[2] = new float4(  -1.0f, 0.0f,  0.0f, -1.0f);
            tangents[3] = new float4(   0.0f, 0.0f, -1.0f, -1.0f);
            
            Vertex vertex = new Vertex();

            int ti = 0;

            // Bottom face
            streams.SetTriangle(ti++, new int3(0, 1, 3));
            streams.SetTriangle(ti++, new int3(1, 2, 3));

            // 3 middle faces starting from back face
            for (int vi = 0; vi < 3; vi++)
            {
                vertex.position = positions[vi];
                vertex.normal = normalize(positions[vi]);
                vertex.tangent = tangents[vi];
                streams.SetVertex(vi, vertex);

                vertex.position = positions[vi + 4];
                vertex.normal = normalize(positions[vi + 4]);
                streams.SetVertex(vi + 4, vertex);

                streams.SetTriangle(ti++, new int3(vi + 0, vi + 5, vi + 1));
                streams.SetTriangle(ti++, new int3(vi + 4, vi + 5, vi + 0));
            }
            // 4th middle face
            vertex.position = positions[3];
            vertex.normal = normalize(positions[3]);
            vertex.tangent = tangents[3];
            streams.SetVertex(3, vertex);

            vertex.position = positions[7];
            vertex.normal = normalize(positions[7]);
            streams.SetVertex(7, vertex);

            streams.SetTriangle(ti++, new int3(3, 4, 0));
            streams.SetTriangle(ti++, new int3(7, 4, 3));

            // Top face
            streams.SetTriangle(ti++, new int3(4, 6, 5));
            streams.SetTriangle(ti++, new int3(7, 6, 4));

            positions.Dispose();
            tangents.Dispose();
        }
    }
}

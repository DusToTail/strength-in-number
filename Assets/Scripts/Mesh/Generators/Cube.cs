using UnityEngine;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace StrengthInNumber.ProceduralMeshes
{
    public struct Cube : IMeshGenerator
    {
        public Bounds Bounds => new Bounds(Vector3.zero, Vector3.one);

        public int VertexCount => 24;

        public int IndexCount => 36;

        public int JobLength => 6;

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            int vi = i * 4;
            int ti = i * 2;
            Sides side = GetSide(i);
            Vertex vertex = new Vertex()
            {
                position = side.position,
                normal = side.normal,
                tangent = side.tangent,
                texCoord0 = float2(0.0f)
            };
            float3 bitangent = normalize(cross(vertex.normal, vertex.tangent.xyz));

            streams.SetVertex(vi, vertex);


            vertex.position += vertex.tangent.xyz;
            vertex.texCoord0.x = 1.0f;
            streams.SetVertex(vi + 1, vertex);

            vertex.position += -bitangent;
            vertex.texCoord0.y = 1.0f;
            streams.SetVertex(vi + 2, vertex);

            vertex.position += -vertex.tangent.xyz;
            vertex.texCoord0.x = 0.0f;
            streams.SetVertex(vi + 3, vertex);


            streams.SetTriangle(ti, new int3(vi, vi + 3, vi + 1));
            streams.SetTriangle(ti + 1, new int3(vi + 3, vi + 2, vi + 1));
        }

        private struct Sides
        {
            public float3 position;
            public float3 normal;
            public float4 tangent;
        }

        private Sides GetSide(int i)
        {
            switch(i)
            {
                case 0:
                {
                    return new Sides()
                    {
                        position = new float3(-0.5f, -0.5f, 0.5f),
                        normal = new float3(0.0f,-1.0f, 0.0f),
                        tangent = new float4(1.0f, 0.0f, 0.0f, -1.0f)
                    };
                }
                case 1:
                {
                    return new Sides()
                    {
                        position = new float3(-0.5f, -0.5f, -0.5f),
                        normal = new float3(0.0f, 0.0f, -1.0f),
                        tangent = new float4(1.0f, 0.0f, 0.0f, -1.0f)
                    };
                }
                case 2:
                {
                    return new Sides()
                    {
                        position = new float3(0.5f, -0.5f, -0.5f),
                        normal = new float3(1.0f, 0.0f, 0.0f),
                        tangent = new float4(0.0f, 0.0f, 1.0f, -1.0f)
                    };
                }
                case 3:
                {
                    return new Sides()
                    {
                        position = new float3(0.5f, -0.5f, 0.5f),
                        normal = new float3(0.0f, 0.0f, 1.0f),
                        tangent = new float4(-1.0f, 0.0f, 0.0f, -1.0f)
                    };
                }
                case 4:
                {
                    return new Sides()
                    {
                        position = new float3(-0.5f, -0.5f, 0.5f),
                        normal = new float3(-1.0f, 0.0f, 0.0f),
                        tangent = new float4(0.0f, 0.0f, -1.0f, -1.0f)
                    };
                    }
                default:
                {
                    return new Sides()
                    {
                        position = new float3(-0.5f, 0.5f, -0.5f),
                        normal = new float3(0.0f, 1.0f, 0.0f),
                        tangent = new float4(1.0f, 0.0f, 0.0f, -1.0f)
                    };
                }
            }
        }
    }
}

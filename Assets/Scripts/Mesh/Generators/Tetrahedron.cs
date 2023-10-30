using UnityEngine;
using Unity.Mathematics;

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

        public int VertexCount => 12;

        public int IndexCount => 12;

        public int JobLength => 4;

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            int vi = i * 3;
            int ti = i;
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

            vertex.position += - bitangent * sqrt(3) / 2 - vertex.tangent.xyz / 2;
            vertex.texCoord0.xy = new float2(0.5f, 1.0f);
            streams.SetVertex(vi + 2, vertex);

            streams.SetTriangle(ti, new int3(vi, vi + 2, vi + 1));
        }

        private struct Sides
        {
            public float3 position;
            public float3 normal;
            public float4 tangent;
        }

        private Sides GetSide(int i)
        {
            switch (i)
            {
                case 0:
                {
                    return new Sides()
                    {
                        position = new float3(0.5f, -sqrt(6) / 12, -sqrt(3) / 6),
                        normal = new float3(0.0f, -1.0f, 0.0f),
                        tangent = new float4(-1.0f, 0.0f, 0.0f, -1.0f)
                    };
                }
                case 1:
                {
                    return new Sides()
                    {
                        position = new float3(-0.5f, -sqrt(6) / 12, -sqrt(3) / 6),
                        normal = normalize(cross(
                            new float3(0.0f, sqrt(6) / 4, 0.0f) - new float3(-0.5f, -sqrt(6) / 12, -sqrt(3) / 6),
                            new float3(1.0f, 0.0f, 0.0f))),
                        tangent = new float4(1.0f, 0.0f, 0.0f, -1.0f)
                    };
                    }
                case 2:
                {
                    return new Sides()
                    {
                        position = new float3(0.5f, -sqrt(6) / 12, -sqrt(3) / 6),
                        normal = normalize(cross(
                            new float3(0.0f, sqrt(6) / 4, 0.0f) - new float3(0.5f, -sqrt(6) / 12, -sqrt(3) / 6),
                            new float3(-0.5f, 0.0f, sqrt(3) / 2))),
                        tangent = new float4(normalize(new float3(-0.5f, 0.0f, sqrt(3) / 2)), -1.0f)
                    };
                }
                default:
                {
                    return new Sides()
                    {
                        position = new float3(0.0f, -sqrt(6) / 12, sqrt(3) / 3),
                        normal = normalize(cross(
                            new float3(0.0f, sqrt(6) / 4, 0.0f) - new float3(0.0f, -sqrt(6) / 12, sqrt(3) / 3),
                            new float3(-0.5f, 0.0f, -sqrt(3) / 2))),
                        tangent = new float4(normalize(new float3(-0.5f, 0.0f, -sqrt(3) / 2)), -1.0f)
                    };
                }
            }
        }
    }
}

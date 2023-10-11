using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using Unity.Collections;
using Unity.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

using static Unity.Mathematics.math;

namespace StrengthInNumber
{
    public static class GeometryUtils
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public float3 position, normal;
            public float4 tangent;
            public float2 texCoord0;
        }
        public const string PrimitivesPath = "Assets/Primitives.asset";

        [MenuItem("Primitives/CreateAsset")]
        public static void CreatePrimitivesAsset()
        {
            AssetDatabase.CreateAsset(Triangle.GenerateTriangleMesh(), PrimitivesPath);
        }

        public static class Triangle
        {
            public static readonly float Height = sqrt(3) / 2;
            public static readonly float CenterToVertex = Height * 2 / 3;
            public static readonly float CenterToEdge = Height / 3;
            public static readonly Bounds Bounds = new Bounds(new Vector3(0f, 0f, CenterToEdge), new Vector3(1f, 1f, Height));
            public static readonly float3[] Vertices = new float3[]
            {
                new float3(-0.5f, 0f, -CenterToEdge),
                new float3(0.5f, 0f, -CenterToEdge),
                new float3(0f, 0f, CenterToVertex),
            };

            public static Mesh GenerateTriangleMesh()
            {
                var meshDataArray = Mesh.AllocateWritableMeshData(1);
                Mesh.MeshData meshData = meshDataArray[0];

                int vertexAttributeCount = 4;
                int vertexCount = 3;
                int triangleIndexCount = 3;

                var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(
                    vertexAttributeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory
                );

                vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3);
                vertexAttributes[1] = new VertexAttributeDescriptor(
                    VertexAttribute.Normal, dimension: 3
                );
                vertexAttributes[2] = new VertexAttributeDescriptor(
                    VertexAttribute.Tangent, dimension: 4
                );
                vertexAttributes[3] = new VertexAttributeDescriptor(
                    VertexAttribute.TexCoord0, dimension: 2
                );

                meshData.SetVertexBufferParams(vertexCount, vertexAttributes);
                vertexAttributes.Dispose();

                NativeArray<Vertex> vertices = meshData.GetVertexData<Vertex>();

                float h0 = 0f, h1 = 1f;

                var vertex = new Vertex
                {
                    normal = up(),
                    tangent = float4(h1, h0, h0, -1f)
                };

                vertex.position = Vertices[0];
                vertex.texCoord0 = h0;
                vertices[0] = vertex;

                vertex.position = Vertices[1];
                vertex.texCoord0 = float2(1f, 0f);
                vertices[1] = vertex;

                vertex.position = Vertices[2];
                vertex.texCoord0 = float2(0.5f, 1f);
                vertices[2] = vertex;

                meshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);
                NativeArray<ushort> triangleIndices = meshData.GetIndexData<ushort>();

                triangleIndices[0] = 0;
                triangleIndices[1] = 2;
                triangleIndices[2] = 1;

                var bounds = Bounds;

                meshData.subMeshCount = 1;
                meshData.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
                {
                    bounds = bounds,
                    vertexCount = vertexCount
                }, MeshUpdateFlags.DontRecalculateBounds);

                Mesh mesh = new Mesh
                {
                    bounds = bounds,
                    name = "Triangle"
                };
                Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

                return mesh;
            }
        }

        public static class Tetrahedron
        {
            public static readonly float Height = sqrt(6) / 3;
            public static readonly float CenterToVertex = Height * 3 / 4;
            public static readonly float CenterToFace = Height / 4;
            public static readonly float Height2D = sqrt(3) / 2;
            public static readonly float Center2DToEdge = Height2D / 3;
            public static readonly float Center2DToVertex = Height2D * 2 / 3;
            public static readonly Bounds Bounds = new Bounds(new Vector3(0f, CenterToFace, Center2DToEdge), new Vector3(1f, Height, Height2D));

            public static readonly float3[] Vertices = new float3[]
            {
                new float3(-0.5f, -CenterToFace, -Center2DToEdge),
                new float3(0.5f, -CenterToFace, -Center2DToEdge),
                new float3(0f, -CenterToFace, Center2DToVertex),
                new float3(0f, CenterToVertex, 0f)
            };

            public static Mesh GenerateTriangleMesh()
            {
                var meshDataArray = Mesh.AllocateWritableMeshData(1);
                Mesh.MeshData meshData = meshDataArray[0];

                int vertexAttributeCount = 4;
                int vertexCount = 4;
                int triangleIndexCount = 12;

                var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(
                    vertexAttributeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory
                );

                vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3);
                vertexAttributes[1] = new VertexAttributeDescriptor(
                    VertexAttribute.Normal, dimension: 3
                );
                vertexAttributes[2] = new VertexAttributeDescriptor(
                    VertexAttribute.Tangent, dimension: 4
                );
                vertexAttributes[3] = new VertexAttributeDescriptor(
                    VertexAttribute.TexCoord0, dimension: 2
                );

                meshData.SetVertexBufferParams(vertexCount, vertexAttributes);
                vertexAttributes.Dispose();

                NativeArray<Vertex> vertices = meshData.GetVertexData<Vertex>();

                //float h0 = 0f, h1 = 1f;

                //var vertex = new Vertex
                //{
                //    normal = up(),
                //    tangent = float4(h1, h0, h0, -1f)
                //};

                //vertex.position = Vertices[0];
                //vertex.texCoord0 = h0;
                //vertices[0] = vertex;

                //vertex.position = Vertices[1];
                //vertex.texCoord0 = float2(1f, 0f);
                //vertices[1] = vertex;

                //vertex.position = Vertices[2];
                //vertex.texCoord0 = float2(0.5f, 1f);
                //vertices[2] = vertex;

                //vertex.position = Vertices[3];
                //vertex.texCoord0 = float2(0.5f, 1f);
                //vertices[2] = vertex;

                meshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);
                NativeArray<ushort> triangleIndices = meshData.GetIndexData<ushort>();

                //triangleIndices[0] = 0;
                //triangleIndices[1] = 2;
                //triangleIndices[2] = 1;
                //triangleIndices[3] = 0;
                //triangleIndices[4] = 2;
                //triangleIndices[5] = 1;
                //triangleIndices[6] = 0;
                //triangleIndices[7] = 2;
                //triangleIndices[8] = 1;
                //triangleIndices[9] = 0;
                //triangleIndices[10] = 2;
                //triangleIndices[11] = 1;

                var bounds = Bounds;

                meshData.subMeshCount = 1;
                meshData.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
                {
                    bounds = bounds,
                    vertexCount = vertexCount
                }, MeshUpdateFlags.DontRecalculateBounds);

                Mesh mesh = new Mesh
                {
                    bounds = bounds,
                    name = "Pyramid"
                };
                Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

                return mesh;
            }
        }

        public static class Square
        {
            public static readonly float CenterToVertex = 1 / SQRT2;
            public static readonly Bounds Bounds = new Bounds(Vector3.zero, Vector3.one);

            public static readonly float3[] Vertices = new float3[]
            {
                new float3(-0.5f, 0f, -0.5f),
                new float3(0.5f, 0f, -0.5f),
                new float3(0.5f, 0f, 0.5f),
                new float3(-0.5f, 0f, 0.5f),
            };
        }

        public static class Cube
        {
            public static readonly float CenterToVertex = sqrt(3) / 2;
            public static readonly Bounds Bounds = new Bounds(Vector3.zero, Vector3.one);

            public static readonly float3[] Vertices = new float3[]
            {
                new float3(-0.5f, -0.5f, -0.5f),
                new float3(0.5f, -0.5f, -0.5f),
                new float3(0.5f, -0.5f, 0.5f),
                new float3(-0.5f, -0.5f, 0.5f),
                new float3(-0.5f, 0.5f, -0.5f),
                new float3(0.5f, 0.5f, -0.5f),
                new float3(0.5f, 0.5f, 0.5f),
                new float3(-0.5f, 0.5f, 0.5f)
            };
        }

        public static class Circle
        {

        }

        public static class Sphere
        {

        }
    }
}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using StrengthInNumber.ProceduralMeshes.Streams;
using System.IO;

namespace StrengthInNumber.ProceduralMeshes
{
    public static class ProceduralMeshes
    {
        public static readonly Mesh Cube = new Mesh()
        {
            name = "Cube"
        };
        public static readonly Mesh Tetrahedron = new Mesh()
        {
            name = "Tetrahedron"
        };
        public const string RelativePath = "Meshes/Primitives/";

        [MenuItem("Primitives/CreateAsset")]
        public static void CreatePrimitivesAsset()
        {
            CreateCube();
            CreateTetrahedron();

            CreateAsset(Cube);
            CreateAsset(Tetrahedron);
        }

        private static void CreateAsset(Mesh mesh)
        {
            string absolutePath = Application.dataPath + '/' + RelativePath;
            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            AssetDatabase.CreateAsset(mesh, "Assets/" + RelativePath + mesh.name + ".asset");
        }

        private static void CreateCube()
        {
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            MeshJob<Cube, SingleStream>.ScheduleParallel(
                Cube, meshData, default
            ).Complete();

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, Cube);
        }

        private static void CreateTetrahedron()
        {
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            MeshJob<Tetrahedron, SingleStream>.ScheduleParallel(
                Tetrahedron, meshData, default
            ).Complete();

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, Tetrahedron);
        }
    }
}
#endif

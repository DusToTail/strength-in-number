#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using StrengthInNumber.ProceduralMeshes.Streams;
using System.IO;

namespace StrengthInNumber.ProceduralMeshes
{
    /// <summary>
    /// Unity Editor only
    /// </summary>
    public static class ProceduralMeshes
    {
        public static Mesh Cube;
        public static Mesh Tetrahedron;
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
            string path = "Assets/" + RelativePath + mesh.name + ".asset";
            if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets)))
            {
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.CreateAsset(mesh, path);
        }

        private static void CreateCube()
        {
            Cube = new Mesh()
            {
                name = "Cube"
            };
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            MeshJob<Cube, SingleStream>.ScheduleParallel(
                Cube, meshData, default
            ).Complete();

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, Cube);
        }

        private static void CreateTetrahedron()
        {
            Tetrahedron = new Mesh()
            {
                name = "Tetrahedron"
            };
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

using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace StrengthInNumber.Builder
{
    public class BuilderAuthoring : MonoBehaviour
    {
        public BuildPrefabAuthoring[] prefabs;

        public class BuilderBaker : Baker<BuilderAuthoring>
        {
            public override void Bake(BuilderAuthoring authoring)
            {
                var self = GetEntity(TransformUsageFlags.None);
                var buffer = AddBuffer<BuildPrefab>(self);

                if(authoring.prefabs == null && authoring.prefabs.Length == 0)
                {
                    return;
                }
                for(int i = 0; i < authoring.prefabs.Length; i++)
                {
                    var prefab = authoring.prefabs[i];
                    buffer.Add(new BuildPrefab()
                    {
                        prefab = GetEntity(prefab.gameObject, prefab.transformUsageFlags),
                    });
                }
            }
        }
    }

    [InternalBufferCapacity(16)]
    public struct BuildPrefab : IBufferElementData
    {
        public Entity prefab;
    }

    public struct BuildEntity : IComponentData
    {
        public int prefabIndex;
        public float3 position;
        public quaternion rotation;
        public float scale;
    }
}

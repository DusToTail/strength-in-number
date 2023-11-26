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
                AddComponent(self, new Builder() { });
                var buffer = AddBuffer<BuildPrefab>(self);

                if(authoring.prefabs == null || authoring.prefabs.Length == 0)
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

    public struct Builder : IComponentData
    {
        public enum GridType
        {
            Square,
            Triangle
        }
        public int prefabIndex;
        public int faceEnum;
        public int2 gridPosition;
        public GridType gridType;
    }

    [InternalBufferCapacity(16)]
    public struct BuildPrefab : IBufferElementData
    {
        public Entity prefab;
    }

    public struct BuildEntity : IComponentData
    {
        public int prefabIndex;
        public int2 position;
        public int faceEnum;
        public Builder.GridType gridType;
    }

    public struct InitilizationTag : IComponentData
    {
    }
}

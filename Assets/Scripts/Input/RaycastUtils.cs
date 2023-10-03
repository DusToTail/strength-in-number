using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Jobs;
using Unity.Burst;
using System.Runtime.CompilerServices;
using System;

namespace StrengthInNumber
{
    [Flags]
    public enum CollisionLayers
    {
        Nothing = 0,
        Selection = 1,
        Ground = 1 << 1,
        Object = 1 << 2
    }

    public static class RaycastUtils
    {
        public static readonly CollisionLayers AllCollisionLayers =
            CollisionLayers.Nothing |
            CollisionLayers.Selection |
            CollisionLayers.Ground |
            CollisionLayers.Object;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt(CollisionLayers layer)
        {
            return (uint)(layer & AllCollisionLayers);
        }

        public static void GetMouseRaycastQuery(EntityManager em, out EntityQuery query)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<MouseRaycast, Input_Mouse_Position>();
            query = builder.Build(em);
            builder.Dispose();
        }

        [BurstCompile]
        public struct RaycastJob : IJobParallelFor
        {
            [ReadOnly] public CollisionWorld world;
            [ReadOnly] public NativeArray<RaycastInput> inputs;
            public NativeArray<RaycastHit> results;

            public unsafe void Execute(int index)
            {
                RaycastHit hit;
                world.CastRay(inputs[index], out hit);
                results[index] = hit;
            }
        }

        public static JobHandle ScheduleBatchRayCast(CollisionWorld world,
            NativeArray<RaycastInput> inputs, NativeArray<RaycastHit> results, JobHandle dependency = default)
        {
            JobHandle rcj = new RaycastJob
            {
                inputs = inputs,
                results = results,
                world = world

            }.Schedule(inputs.Length, 4, dependency);
            return rcj;
        }

        public static void SingleRayCast(CollisionWorld world, RaycastInput input,
        ref RaycastHit result, JobHandle dependency = default)
        {
            var rayCommands = new NativeArray<RaycastInput>(1, Allocator.TempJob);
            var rayResults = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            rayCommands[0] = input;
            var handle = ScheduleBatchRayCast(world, rayCommands, rayResults, dependency);
            handle.Complete();
            result = rayResults[0];
            rayCommands.Dispose();
            rayResults.Dispose();
        }
    }
}

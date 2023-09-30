using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using RaycastHit = Unity.Physics.RaycastHit;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial class MouseRaycastSystem : SystemBase
    {
        private Camera _camera;

        protected override void OnCreate()
        {
            RequireForUpdate<MouseRaycastData>();
            RequireForUpdate<PlayerMouse>();
        }

        protected override void OnStartRunning()
        {
            _camera = Camera.main;
        }

        protected override void OnStopRunning()
        {
            _camera = null;
        }

        protected override void OnUpdate()
        {
            EntityManager.CompleteDependencyBeforeRO<PhysicsWorldSingleton>();

            var castData = SystemAPI.GetSingletonRW<MouseRaycastData>();
            float2 mousePosition = SystemAPI.GetSingleton<PlayerMouse>().position;

            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0f));
            float3 start = ray.origin;
            float3 end = ray.GetPoint(castData.ValueRO.castDistance);
            var filter = castData.ValueRO.filter;
            var hit = Raycast(start, end, filter);

            Color color = Color.red;
            if(hit != Entity.Null)
            {
                if (EntityManager.HasComponent<SelectableTag>(hit))
                {
                    color = Color.green;
                }
                else
                {
                    color = Color.yellow;
                }
            }
            Debug.DrawLine(start, end, color);
            castData.ValueRW.hit = hit;
        }

        public Entity Raycast(float3 from, float3 to, CollisionFilter filter)
        {
            var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            RaycastInput input = new RaycastInput()
            {
                Start = from,
                End = to,
                Filter = filter
            };

            RaycastHit hit = new RaycastHit();
            SingleRayCast(world, input, ref hit);
            return hit.Entity;
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
            NativeArray<RaycastInput> inputs, NativeArray<RaycastHit> results)
        {
            JobHandle rcj = new RaycastJob
            {
                inputs = inputs,
                results = results,
                world = world

            }.Schedule(inputs.Length, 4);
            return rcj;
        }

        public static void SingleRayCast(CollisionWorld world, RaycastInput input,
        ref RaycastHit result)
        {
            var rayCommands = new NativeArray<RaycastInput>(1, Allocator.TempJob);
            var rayResults = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            rayCommands[0] = input;
            var handle = ScheduleBatchRayCast(world, rayCommands, rayResults);
            handle.Complete();
            result = rayResults[0];
            rayCommands.Dispose();
            rayResults.Dispose();
        }
    }
}

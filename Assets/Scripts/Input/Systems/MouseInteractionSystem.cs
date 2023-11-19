using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber.Input
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProcessRaycastSystem))]
    public partial struct MouseInteractionSystem : ISystem
    {
        private EntityQuery _mouseQ;
        private EntityQuery _hoverredQ;
        private EntityQuery _selectedQ;
        private Entity _hoverred;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<Mouse>()
                .WithAllRW<RaycastInput, RaycastOutput>();
                _mouseQ = builder.Build(ref state);
                builder.Dispose();
            }

            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Hoverred>();
                _hoverredQ = builder.Build(ref state);
                builder.Dispose();
            }

            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selected>();
                _selectedQ = builder.Build(ref state);
                builder.Dispose();
            }

            state.RequireForUpdate(_mouseQ);
        }

        public void OnUpdate(ref SystemState state)
        {
            var mouse = _mouseQ.GetSingletonRW<Mouse>();
            var output = _mouseQ.GetSingletonRW<RaycastOutput>().ValueRO;
            var input = _mouseQ.GetSingletonRW<RaycastInput>();

            bool select = mouse.ValueRO.select;
            bool deselect = mouse.ValueRO.deselect;
            _hoverred = output.hit.Entity;

            // Prepare input for next frame mouse raycast
            float2 mousePosition = mouse.ValueRO.position;
            var ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y));
            input.ValueRW.start = ray.origin;
            input.ValueRW.end = ray.GetPoint(100f);

            DebugDrawRay(input.ValueRO.start, input.ValueRO.end, 
                (_hoverred == Entity.Null || !state.EntityManager.HasComponent<Selected>(_hoverred) || !state.EntityManager.HasComponent<Hoverred>(_hoverred)) ? 
                    Color.red : 
                    (state.EntityManager.IsComponentEnabled<Selected>(_hoverred) ?
                        Color.green :
                        (state.EntityManager.IsComponentEnabled<Hoverred>(_hoverred) ?
                            Color.yellow : Color.red)));

            // Process output from this frame mouse raycast
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            UnhoverAll(ref state, ref ecb);
            if (deselect)
            {
                mouse.ValueRW.deselect = false;
                DeselectAll(ref state, ref ecb);
            }
            HoverOne(ref state, ref ecb);
            if (select)
            {
                mouse.ValueRW.select = false;
                DeselectAll(ref state, ref ecb);
                SelectOne(ref state, ref ecb);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private void HoverOne(ref SystemState state, ref EntityCommandBuffer ecb)
        {
            if(_hoverred != Entity.Null && state.EntityManager.HasComponent<Hoverred>(_hoverred))
            {
                ecb.SetComponentEnabled<Hoverred>(_hoverred, true);
            }
        }

        private void UnhoverAll(ref SystemState state, ref EntityCommandBuffer ecb)
        {
            new UnhoverJob()
            {
                hoverred = _hoverred,
                ecb = ecb.AsParallelWriter()
            }.ScheduleParallel(_hoverredQ, state.Dependency).Complete();
        }

        private void SelectOne(ref SystemState state, ref EntityCommandBuffer ecb)
        {
            if (_hoverred != Entity.Null && state.EntityManager.HasComponent<Selected>(_hoverred))
            {
                ecb.SetComponentEnabled<Selected>(_hoverred, true);
            }
        }

        private void DeselectAll(ref SystemState state, ref EntityCommandBuffer ecb)
        {
            new DeselectJob()
            {
                hoverred = _hoverred,
                ecb = ecb.AsParallelWriter()
            }.ScheduleParallel(_selectedQ, state.Dependency).Complete();
        }

        /// <summary>
        /// The function is a wrapper of UnityEngine.Debug.DrawRay(). Implementation is wrapped in UNITY_EDITOR preprocessor condition
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        partial void DebugDrawRay(Vector3 start, Vector3 end, Color color);
        /// <summary>
        /// The function is a wrapper of UnityEngine.Debug.Log(). Implementation is wrapped in UNITY_EDITOR preprocessor condition
        /// </summary>
        /// <param name="message"></param>
        partial void DebugLog(string message);

#if UNITY_EDITOR
        partial void DebugDrawRay(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawRay(start, end, color);
        }
        partial void DebugLog(string message)
        {
            Debug.Log(message);
        }
#endif


        [BurstCompile]
        partial struct UnhoverJob : IJobEntity
        {
            public Entity hoverred;
            public EntityCommandBuffer.ParallelWriter ecb;

            public void Execute(Entity e, [ChunkIndexInQuery] int sortKey)
            {
                ecb.SetComponentEnabled<Hoverred>(sortKey, e, e == hoverred);
            }
        }

        [BurstCompile]
        partial struct DeselectJob : IJobEntity
        {
            public Entity hoverred;
            public EntityCommandBuffer.ParallelWriter ecb;

            public void Execute(Entity e, [ChunkIndexInQuery] int sortKey)
            {
                ecb.SetComponentEnabled<Selected>(sortKey, e, false);
            }
        }
    }
}

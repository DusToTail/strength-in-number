using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Physics;
using UnityEngine;

using RaycastHit = Unity.Physics.RaycastHit;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(Physics_QueryAfter_SystemGroup))]
    public partial class Mouse_RaycastSystem : SystemBase
    {
        private Camera _camera;
        private EntityQuery _raycastQuery;
        private EntityQuery _mouseQuery;
        private Entity _raycast;
        private Entity _mouse;

        protected override void OnCreate()
        {
            RaycastUtils.GetMouseRaycastQuery(EntityManager, out _raycastQuery);
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Input_Mouse_Position>();
            _mouseQuery = builder.Build(EntityManager);
            builder.Dispose();

            RequireForUpdate(_raycastQuery);
            RequireForUpdate(_mouseQuery);
        }

        protected override void OnDestroy()
        {
            _raycastQuery.Dispose();
            _mouseQuery.Dispose();
        }

        protected override void OnStartRunning()
        {
            _camera = Camera.main;

            // Cant use GetSingletonEntity due to MouseRaycast : IEnableComponent
            var arr = _raycastQuery.ToEntityArray(Allocator.Temp);
            _raycast = arr[0];
            arr.Dispose();

            _mouse = _mouseQuery.GetSingletonEntity();
        }

        protected override void OnStopRunning()
        {
            _camera = null;
            _raycast = Entity.Null;
            _mouse = Entity.Null;
        }

        protected override void OnUpdate()
        {
            EntityManager.CompleteDependencyBeforeRO<PhysicsWorldSingleton>();

            var raycastRW = SystemAPI.GetComponentRW<MouseRaycast>(_raycast);
            float2 positionRO = SystemAPI.GetComponentRO<Input_Mouse_Position>(_mouse).ValueRO.position;

            var ray = _camera.ScreenPointToRay(new Vector3(positionRO.x, positionRO.y, 0f));
            float3 start = ray.origin;
            float3 end = ray.GetPoint(raycastRW.ValueRO.castDistance);
            var filter = raycastRW.ValueRO.filter;

            Color color = Color.red;
            if(Raycast(start, end, filter, ref raycastRW.ValueRW.hit))
            {
                if (EntityManager.HasComponent<SelectableTag>(raycastRW.ValueRO.hit.Entity))
                {
                    color = Color.green;
                }
                else
                {
                    color = Color.yellow;
                }
            }
            Debug.DrawLine(start, end, color);
        }

        public bool Raycast(float3 from, float3 to, CollisionFilter filter, ref RaycastHit hit, JobHandle dependency = default)
        {
            var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            RaycastInput input = new RaycastInput()
            {
                Start = from,
                End = to,
                Filter = filter
            };

            RaycastUtils.SingleRayCast(world, input, ref hit, dependency);
            if(hit.Entity != Entity.Null)
            {
                return true;
            }
            return false;
        }
    }
}

using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(Physics_QueryAfter_SystemGroup))]
    [UpdateAfter(typeof(Mouse_RaycastSystem))]
    public partial struct Mouse_SelectionSystem : ISystem, ISystemStartStop
    {
        private EntityQuery _raycastQuery;
        private EntityQuery _mouseQuery;
        private Entity _raycast;
        private Entity _mouse;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            RaycastUtils.GetMouseRaycastQuery(state.EntityManager, out _raycastQuery);
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Input_Mouse_Select, Input_Mouse_Deselect>();
            _mouseQuery = builder.Build(state.EntityManager);
            builder.Dispose();

            state.RequireForUpdate(_raycastQuery);
            state.RequireForUpdate(_mouseQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _raycastQuery.Dispose();
            _mouseQuery.Dispose();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            // Cant use GetSingletonEntity due to MouseRaycast : IEnableComponent
            var arr = _raycastQuery.ToEntityArray(Allocator.Temp);
            _raycast = arr[0];
            arr.Dispose();

            _mouse = _mouseQuery.GetSingletonEntity();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            _raycast = Entity.Null;
            _mouse = Entity.Null;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.WorldUnmanaged.EntityManager;
            var select = em.GetComponentData<Input_Mouse_Select>(_mouse);
            var deselect = em.GetComponentData<Input_Mouse_Deselect>(_mouse);
            bool selectTriggered = select.triggered;
            bool deselectTriggered = deselect.triggered;

            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SelectedFlag>();
            EntityQuery selectedQuery = builder.Build(em);
            builder.Dispose();

            if (selectTriggered)
            {
                // TODO: Shift click and make into separate function and maybe into scheduled jobs.
                TryDeselect(em, selectedQuery);

                var mouseRaycastData = em.GetComponentData<MouseRaycast>(_raycast);
                var hoverred = mouseRaycastData.hit;
                TrySelect(em, hoverred);

                // Consume the trigger
                select.triggered = false;
            }

            if(deselectTriggered)
            {
                TryDeselect(em, selectedQuery);

                // Consume the trigger
                deselect.triggered = false;
            }

            selectedQuery.Dispose();
        }

        private bool TryDeselect(EntityManager em, EntityQuery query)
        {
            if(!query.IsEmpty)
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    em.SetComponentEnabled<SelectedFlag>(entity, false);
                    FixedString64Bytes name;
                    em.GetName(entity, out name);
                    Debug.Log($"MouseSelection: Deselect {name}({entity.Index})");
                }
                entities.Dispose();
                return true;
            }
            return false;
        }

        private bool TrySelect(EntityManager em, Entity entity)
        {
            if (entity != Entity.Null &&
                em.HasComponent<SelectableTag>(entity) == true &&
                em.IsComponentEnabled<SelectedFlag>(entity) == false)
            {
                em.SetComponentEnabled<SelectedFlag>(entity, true);
                FixedString64Bytes name;
                em.GetName(entity, out name);
                Debug.Log($"MouseSelection: Select {name}({entity.Index})");
                return true;
            }
            return false;
        }
    }
}

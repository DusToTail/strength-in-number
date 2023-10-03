using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(Physics_QueryBefore_SystemGroup))]
    [UpdateAfter(typeof(MouseRaycastSystem))]
    public partial struct MouseSelectionSystem : ISystem, ISystemStartStop
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
            var em = state.EntityManager;
            bool selectTriggered = em.GetComponentData<Input_Mouse_Select>(_mouse).triggered;
            bool deselectTriggered = em.GetComponentData<Input_Mouse_Deselect>(_mouse).triggered;

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
            }

            if(deselectTriggered)
            {
                TryDeselect(em, selectedQuery);
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
                return true;
            }
            return false;
        }
    }
}

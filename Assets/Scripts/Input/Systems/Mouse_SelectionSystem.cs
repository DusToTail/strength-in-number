using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

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
        public struct HoverJob : IJobParallelFor
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> entities;
            [ReadOnly] public EntityManager em;
            [ReadOnly] public Entity hoverred;

            public void Execute(int index)
            {
                var e = entities[index];
                em.SetComponentEnabled<HoverredFlag>(e, e == hoverred);
            }
        }

        [BurstCompile]
        public struct SelectJob : IJobParallelFor
        {
            [DeallocateOnJobCompletion][ReadOnly] public NativeArray<Entity> entities;
            [ReadOnly] public EntityManager em;
            [ReadOnly] public Entity hoverred;

            public bool selectTriggered;
            public bool deselectTriggered;

            public void Execute(int index)
            {
                var e = entities[index];

                if(deselectTriggered)
                {
                    em.SetComponentEnabled<SelectedFlag>(e, false);
                }
                if (selectTriggered)
                {
                    em.SetComponentEnabled<SelectedFlag>(e, e == hoverred);
                }
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.WorldUnmanaged.EntityManager;
            var select = em.GetComponentData<Input_Mouse_Select>(_mouse);
            var deselect = em.GetComponentData<Input_Mouse_Deselect>(_mouse);
            bool selectTriggered = select.triggered;
            bool deselectTriggered = deselect.triggered;
            var mouseRaycastData = em.GetComponentData<MouseRaycast>(_raycast);
            var hoverred = mouseRaycastData.hit;
            
            // Hover evaluation
            EntityQueryBuilder hover = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<HoverredFlag>()
                .WithOptions(EntityQueryOptions.IncludeDisabledEntities);
            EntityQuery hoverQuery = hover.Build(em);
            hover.Dispose();

            var hoverredArr = hoverQuery.ToEntityArray(Allocator.TempJob);
            new HoverJob()
            {
                entities = hoverredArr,
                em = em,
                hoverred = hoverred.Entity
            }.Schedule(hoverredArr.Length, 32, state.Dependency).Complete();

            // Selection evaluation
            EntityQueryBuilder selectBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SelectedFlag>()
                .WithOptions(EntityQueryOptions.IncludeDisabledEntities); ;
            EntityQuery selectQuery = selectBuilder.Build(em);
            selectBuilder.Dispose();

            if (selectTriggered || deselectTriggered)
            {
                var selectArr = selectQuery.ToEntityArray(Allocator.TempJob);
                new SelectJob()
                {
                    entities = selectArr,
                    em = em,
                    hoverred = hoverred.Entity,
                    selectTriggered = selectTriggered,
                    deselectTriggered = deselectTriggered
                }.Schedule(selectArr.Length, 32, state.Dependency).Complete();
            }

            // Consume trigger
            if (selectTriggered)
                select.triggered = false;
            if (deselectTriggered)
                deselect.triggered = false;
            

            selectQuery.Dispose();
            hoverQuery.Dispose();
        }
    }
}

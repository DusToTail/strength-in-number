using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MouseRaycastSystem))]
    public partial struct MouseSelectionSystem : ISystem, ISystemStartStop
    {
        private Entity _player;
        private Entity _mouseRaycast;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
            state.RequireForUpdate<MouseRaycastData>();
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            EntityQueryBuilder playerBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerTag>();
            EntityQuery playerQuery = playerBuilder.Build(state.WorldUnmanaged.EntityManager);
            _player = playerQuery.GetSingletonEntity();
            playerBuilder.Dispose();
            playerQuery.Dispose();

            EntityQueryBuilder mouseRaycastBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<MouseRaycastData>();
            EntityQuery mouseRaycastQuery = mouseRaycastBuilder.Build(state.WorldUnmanaged.EntityManager);
            _mouseRaycast = mouseRaycastQuery.GetSingletonEntity();
            mouseRaycastBuilder.Dispose();
            mouseRaycastQuery.Dispose();
        }
        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            _player = Entity.Null;

            _mouseRaycast = Entity.Null;
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.WorldUnmanaged.EntityManager;
            bool selectTriggered = em.IsComponentEnabled<PlayerSelect>(_player);
            bool deselectTriggered = em.IsComponentEnabled<PlayerDeselect>(_player);

            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<SelectedTag>();
            EntityQuery query = builder.Build(em);

            if (selectTriggered)
            {
                // TODO: Shift click and make into separate function and maybe into scheduled jobs. This will be structural change though...
                if (!query.IsEmpty)
                {
                    var entities = query.ToEntityArray(Allocator.Temp);
                    foreach (var entity in entities)
                    {
                        em.RemoveComponent<SelectedTag>(entity);
                    }
                    entities.Dispose();
                }

                var mouseRaycastData = em.GetComponentData<MouseRaycastData>(_mouseRaycast);
                var hoverEntity = mouseRaycastData.hit;

                if (hoverEntity != Entity.Null &&
                    em.HasComponent<SelectableTag>(hoverEntity) &&
                    !em.HasComponent<SelectedTag>(hoverEntity))
                {
                    em.AddComponent<SelectedTag>(hoverEntity);
                }
            }

            if(deselectTriggered)
            {
                if(!query.IsEmpty)
                {
                    var entities = query.ToEntityArray(Allocator.Temp);
                    foreach(var entity in entities)
                    {
                        em.RemoveComponent<SelectedTag>(entity);
                    }
                    entities.Dispose();
                }
            }

            builder.Dispose();
            query.Dispose();
        }
    }
}

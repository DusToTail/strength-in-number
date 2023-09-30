using Unity.Entities;
using Unity.Collections;
using UnityEngine;

namespace StrengthInNumber
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MouseRaycastSystem))]
    public partial struct MouseSelectionSystem : ISystem, ISystemStartStop
    {
        private Entity _player;
        private Entity _mouseRaycast;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
            state.RequireForUpdate<MouseRaycastData>();
        }
        public void OnDestroy(ref SystemState state)
        {
        }
        public void OnStartRunning(ref SystemState state)
        {
            EntityQuery playerQuery = state.GetEntityQuery(typeof(PlayerTag));
            _player = playerQuery.GetSingletonEntity();

            EntityQuery mouseRaycastQuery = state.GetEntityQuery(typeof(MouseRaycastData));
            _mouseRaycast = mouseRaycastQuery.GetSingletonEntity();
        }
        public void OnStopRunning(ref SystemState state)
        {
            _player = Entity.Null;

            _mouseRaycast = Entity.Null;
        }
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
                        Debug.Log($"Deselect {em.GetName(entity)}");
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
                    Debug.Log($"Select {em.GetName(hoverEntity)}");
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
                        Debug.Log($"Deselect {em.GetName(entity)}");
                    }
                    entities.Dispose();
                }
            }

            builder.Dispose();
            query.Dispose();
        }
    }
}

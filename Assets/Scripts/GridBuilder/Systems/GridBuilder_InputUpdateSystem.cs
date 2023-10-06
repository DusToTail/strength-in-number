using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

using BufferElement = StrengthInNumber.GridBuilder.GridBuilder_GridBufferElement;
using BufferSettings = StrengthInNumber.GridBuilder.GridBuilder_GridBufferSettings;

namespace StrengthInNumber.GridBuilder
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(GridBuilder_GridDebugSystem))]
    public partial struct GridBuilder_InputUpdateSystem : ISystem, ISystemStartStop
    {
        private BufferLookup<BufferElement> _gridLookup;

        private EntityQuery _gridBuilderQuery;
        private Entity _gridBuilder;

        private EntityQuery _raycastQuery;
        private Entity _raycast;

        private EntityQuery _mouseQuery;
        private Entity _mouse;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var em = state.WorldUnmanaged.EntityManager;
            GridBuilderUtils.GetQuery(em, out _gridBuilderQuery);
            GridBuilderUtils.GetBufferLookup(ref state, out _gridLookup, false);
            state.RequireForUpdate(_gridBuilderQuery);


            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Input_Mouse_Select, Input_Mouse_Deselect>();
            _mouseQuery = builder.Build(state.EntityManager);
            builder.Dispose();
            RaycastUtils.GetMouseRaycastQuery(em, out _raycastQuery);
            state.RequireForUpdate(_raycastQuery);
            state.RequireForUpdate(_mouseQuery);
        }

        [BurstCompile]
        public struct UpdateBufferStateJob : IJobParallelFor
        {
            public NativeArray<BufferElement> buffer;

            public int2 gridSize;
            public float2 gridCenter;

            public float2 cellSize;

            public bool selectTriggered;
            public bool deselectTriggered;
            public float2 position;

            [BurstCompile]
            public void Execute(int index)
            {
                var temp = buffer[index];
                bool selected = temp.selected;
                bool hoverred = temp.hoverred;

                var gridPos = GridBuilderUtils.WorldToGrid(
                    position,
                    gridCenter,
                    gridSize,
                    cellSize,
                    false);

                if (gridPos.Equals(new int2(-1)))
                {
                    hoverred = false;
                }
                else
                {
                    hoverred = index == GridBuilderUtils.GridToIndex(gridPos, gridSize.x);
                }

                if (deselectTriggered)
                {
                    selected = false;
                }
                if (selectTriggered)
                {
                    selected = hoverred;
                }

                temp.hoverred = hoverred;
                temp.selected = selected;
                buffer[index] = temp;
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _gridLookup.Update(ref state);

            if (_gridLookup.TryGetBuffer(_gridBuilder, out var buffer))
            {
                var em = state.WorldUnmanaged.EntityManager;
                var select = em.GetComponentData<Input_Mouse_Select>(_mouse);
                var deselect = em.GetComponentData<Input_Mouse_Deselect>(_mouse);
                bool selectTriggered = select.triggered;
                bool deselectTriggered = deselect.triggered;
                var mouseRaycastData = em.GetComponentData<MouseRaycast>(_raycast);
                var hitPosition = mouseRaycastData.hit.Position.xz;
                if (mouseRaycastData.hit.Entity == Entity.Null)
                {
                    hitPosition = new float2(float.MinValue);
                }

                BufferSettings settings;
                GridBuilderUtils.GetBufferSettings(state.EntityManager, _gridBuilder, out settings);

                int length = settings.gridSize.x * settings.gridSize.y;

                new UpdateBufferStateJob()
                {
                    buffer = buffer.AsNativeArray(),
                    gridSize = settings.gridSize,
                    gridCenter = settings.gridCenter,
                    cellSize = settings.cellSize,
                    selectTriggered = selectTriggered,
                    deselectTriggered = deselectTriggered,
                    position = hitPosition
                }.Schedule(length, 1, state.Dependency).Complete();
            }
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            _gridBuilder = _gridBuilderQuery.GetSingletonEntity();

            // Cant use GetSingletonEntity due to MouseRaycast : IEnableComponent
            var arr = _raycastQuery.ToEntityArray(Allocator.Temp);
            _raycast = arr[0];
            arr.Dispose();

            _mouse = _mouseQuery.GetSingletonEntity();
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state)
        {
            _gridBuilder = Entity.Null;
            _raycast = Entity.Null;
            _mouse = Entity.Null;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _gridBuilderQuery.Dispose();
            _raycastQuery.Dispose();
            _mouseQuery.Dispose();
        }
    }
}

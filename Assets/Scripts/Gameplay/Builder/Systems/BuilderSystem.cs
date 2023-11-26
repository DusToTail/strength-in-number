using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using StrengthInNumber.Grid;
using StrengthInNumber.Input;
using StrengthInNumber.Entities;

namespace StrengthInNumber.Builder
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProcessRaycastSystem))]
    public partial struct BuilderSystem : ISystem
    {
        private BufferLookup<BuildPrefab> _prefabsLookup;
        private DynamicBuffer<BuildPrefab> _prefabs;
        private EntityQuery _builderQ;
        private EntityQuery _mouseQ;
        private EntityQuery _keyboardQ;
        private EntityQuery _gridQ;

        private RefRW<Mouse> _mouse;
        private RefRW<Keyboard> _keyboard;
        private RefRW<SquareGrid> _grid;
        private RefRW<Builder> _builder;

        private bool _active;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAllRW<Builder>();
                _builderQ = builder.Build(ref state);
                builder.Dispose();
            }
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAllRW<Mouse>()
                    .WithAllRW<RaycastInput, RaycastOutput>();
                _mouseQ = builder.Build(ref state);
                builder.Dispose();
            }
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAllRW<Keyboard>();
                _keyboardQ = builder.Build(ref state);
                builder.Dispose();
            }
            {
                var builder = new EntityQueryBuilder(Allocator.Temp)
                    .WithAllRW<SquareGrid>();
                _gridQ = builder.Build(ref state);
                builder.Dispose();
            }
            _prefabsLookup = state.GetBufferLookup<BuildPrefab>(true);
            state.RequireForUpdate(_builderQ);
            state.RequireForUpdate(_mouseQ);
            state.RequireForUpdate(_keyboardQ);
            state.RequireForUpdate(_gridQ);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _prefabsLookup.Update(ref state);
            _prefabsLookup.TryGetBuffer(_builderQ.GetSingletonEntity(), out _prefabs);

            _keyboard = _keyboardQ.GetSingletonRW<Keyboard>();

            if(_keyboard.ValueRO.cancel)
            {
                _keyboard.ValueRW.cancel = false;
                _active = !_active;
            }

            if (!_active) { return; }

            _builder = _builderQ.GetSingletonRW<Builder>();
            int prefabCount = _prefabs.Length;

            // Prefab selection
            if(_keyboard.ValueRO.up)
            {
                _keyboard.ValueRW.up = false;
                _builder.ValueRW.prefabIndex++;
                _builder.ValueRW.prefabIndex %= prefabCount;
            }
            if(_keyboard.ValueRO.left)
            {
                _keyboard.ValueRW.left = false;
                _builder.ValueRW.prefabIndex--;
                if (_builder.ValueRW.prefabIndex < 0)
                {
                    _builder.ValueRW.prefabIndex = prefabCount - 1;
                }
            }
            if (_keyboard.ValueRO.down)
            {
                _keyboard.ValueRW.down = false;
                _builder.ValueRW.prefabIndex--;
                if(_builder.ValueRW.prefabIndex < 0)
                {
                    _builder.ValueRW.prefabIndex = prefabCount - 1;
                }
            }
            if (_keyboard.ValueRO.right)
            {
                _keyboard.ValueRW.right = false;
                _builder.ValueRW.prefabIndex++;
                _builder.ValueRW.prefabIndex %= prefabCount;
            }

            _mouse = _mouseQ.GetSingletonRW<Mouse>();
            _grid = _gridQ.GetSingletonRW<SquareGrid>();
            // Position selection
            {
                var raycastOutput = _mouseQ.GetSingletonRW<RaycastOutput>();
                var position = raycastOutput.ValueRO.hit.Position;
                _builder.ValueRW.gridPosition = _grid.ValueRO.WorldToGrid(position, true);
            }

            // Rotation selection
            if (_mouse.ValueRO.scroll > 0)
            {
                _builder.ValueRW.faceEnum++;
                _builder.ValueRW.faceEnum %= 5;
            }
            else if(_mouse.ValueRO.scroll < 0)
            {
                _builder.ValueRW.faceEnum--;
                if(_builder.ValueRO.faceEnum < 0)
                {
                    _builder.ValueRW.faceEnum = 4;
                }
            }
            
            // Build
            if (_mouse.ValueRO.select)
            {
                _mouse.ValueRW.select = false;
                var e = state.EntityManager.CreateEntity();

                var prefab = _prefabs[_builder.ValueRO.prefabIndex];
                quaternion rot = quaternion.identity;
                float3 position = default;

                if(state.EntityManager.HasComponent<Cube>(prefab.prefab))
                {
                    position = _grid.ValueRO.GridToWorld(_builder.ValueRO.gridPosition.x, _builder.ValueRO.gridPosition.y);
                    position.y +=_grid.ValueRO.CellSize / 2f;
                    if (_builder.ValueRO.faceEnum != 0)
                    {
                        int2 face2D = SquareGridUtils.ToInt2((SquareGridUtils.Faces)_builder.ValueRO.faceEnum);
                        float3 face3D = math.normalize(new float3(face2D.x, 0f, face2D.y));
                        rot = quaternion.LookRotation(face3D, new float3(0f, 1f, 0f));
                    }
                }

                state.EntityManager.AddComponentData(e, new BuildEntity()
                {
                    prefabIndex = _builder.ValueRO.prefabIndex,
                    position = position,
                    rotation = rot,
                    scale = 1f
                });
            }
        }
    }
}

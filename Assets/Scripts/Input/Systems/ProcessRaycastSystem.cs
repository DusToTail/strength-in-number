using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Physics;

namespace StrengthInNumber.Input
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ProcessRaycastSystem : ISystem
    {
        private EntityQuery _query;
        private EntityQuery _worldQ;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            {
                var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<RaycastInput, RaycastOutput>();
                _query = builder.Build(ref state);
                builder.Dispose();
            }
            {
                var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
                _worldQ = builder.Build(ref state);
                builder.Dispose();
            }
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.CompleteDependencyBeforeRO<PhysicsWorldSingleton>();
            var world = _worldQ.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            int count = _query.CalculateEntityCount();
            
            var inputs = new NativeArray<Unity.Physics.RaycastInput>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var results = new NativeArray<Unity.Physics.RaycastHit>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var myInputs = _query.ToComponentDataArray<RaycastInput>(Allocator.TempJob);
            var myOutputs = new NativeArray<RaycastOutput>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            new CopyInput()
            {
                myInputs = myInputs,
                inputs = inputs
            }.Schedule(count, 32).Complete();

            RaycastUtils.ScheduleBatchRayCast(world, inputs, results).Complete();

            new CopyOutput()
            {
                outputs = myOutputs,
                results = results
            }.Schedule(count, 32).Complete();

            _query.CopyFromComponentDataArray(myOutputs);

            myInputs.Dispose();
            inputs.Dispose();
            results.Dispose();
            myOutputs.Dispose();
        }

        struct CopyInput : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<RaycastInput> myInputs;
            [WriteOnly]
            public NativeArray<Unity.Physics.RaycastInput> inputs;
            public void Execute(int index)
            {
                inputs[index] = myInputs[index];
            }
        }

        struct CopyOutput : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<RaycastHit> results;
            [WriteOnly]
            public NativeArray<RaycastOutput> outputs;

            public void Execute(int index)
            {
                outputs[index] = new RaycastOutput()
                {
                    hit = results[index]
                };
            }
        }
    }
}

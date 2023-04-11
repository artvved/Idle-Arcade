using System.Collections.Generic;
using DefaultNamespace;
using Game.Component;
using Game.Component.Time;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System.Timing
{
    public class SpawnCustomerTickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<TickComponent> tickPool = default;
        private readonly EcsPoolInject<CustomerSpawnComponent> customerSpawnerPool = default;
        readonly EcsPoolInject<QueueComponent> queuePool = default;


        private readonly EcsCustomInject<Fabric> fabric = default;
        private readonly EcsCustomInject<StaticData> data = default;
        private readonly EcsCustomInject<SceneData> sceneData = default;


        private EcsFilter tickFilter;
        private EcsFilter queueFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            var newEntity = world.NewEntity();
            customerSpawnerPool.Value.Add(newEntity);
            tickPool.Value.Add(newEntity).FinalTime = data.Value.CustomerTime;

            tickFilter = world.Filter<TickComponent>().Inc<CustomerSpawnComponent>().End();
            queueFilter = world.Filter<QueueComponent>().Inc<FetusTableTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in tickFilter)
            {
                ref var customerSpawnComponent = ref customerSpawnerPool.Value.Get(entity);
                if (customerSpawnComponent.CustomersCount>=data.Value.MaxCustomers)
                    continue;
                
                ref var component = ref tickPool.Value.Get(entity);

                if (component.CurrentTime >= component.FinalTime)
                {
                   
                    foreach (var ent in queueFilter)
                    {
                        ref var tableQueue =ref queuePool.Value.Get(ent).Value;
                        
                        var newCustomer=fabric.Value.InstantiateCustomer(sceneData.Value.CustomerSpawnPlace.position);
                        tableQueue.Add(world.PackEntity(newCustomer));
                        component.CurrentTime = 0;
                        customerSpawnComponent.CustomersCount++;
                    }
                    
                }

                component.CurrentTime += Time.deltaTime;
            }
        }
    }
}
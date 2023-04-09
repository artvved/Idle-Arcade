using DefaultNamespace;
using Game.Component;
using Game.Component.Time;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.System.Timing
{
    public class SpawnFetusTickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<TickComponent> tickPool = default;
        private readonly EcsPoolInject<PlantComponent> plantPool = default;
        private readonly EcsPoolInject<PlantViewComponent> plantViewPool = default;

        private readonly EcsCustomInject<Fabric> fabric = default;

        private EcsFilter tickFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            tickFilter = world.Filter<TickComponent>().Inc<PlantComponent>().Inc<PlantViewComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in tickFilter)
            {
                ref var plantComponent = ref plantPool.Value.Get(entity);
                if (plantComponent.HasFetus)
                    continue;
                
                ref var component = ref tickPool.Value.Get(entity);

                if (component.CurrentTime >= component.FinalTime)
                {
                    var fetusPlace = plantViewPool.Value.Get(entity).Value.FetusPlace;
                    var newFetus=fabric.Value.InstantiateFetus(fetusPlace);
                    plantComponent.Fetus=world.PackEntity(newFetus);
                    plantComponent.HasFetus = true;
                    component.CurrentTime = 0;
                }

                component.CurrentTime += Time.deltaTime;
            }
        }
    }
}
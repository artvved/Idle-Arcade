using DefaultNamespace;
using DefaultNamespace.Game.Component.Actions;
using Game.Component;
using Game.Mono;
using Game.Service;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.System
{
    public class HarvestSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private EcsPoolInject<StackComponent> stackPool = default;
        private EcsPoolInject<PlantComponent> plantPool = default;

        private EcsPoolInject<HarvestingComponent> harvestingPool = default;
        private EcsCustomInject<MovementService> serv = default;

      
        
        private EcsFilter filter;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            
            filter = world.Filter<PlayerTag>().Inc<HarvestingComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in filter)
            {
                harvestingPool.Value.Get(unit).Target.Unpack(world, out int plant);
                
                ref var stackComponent = ref stackPool.Value.Get(unit);

                if (stackComponent.CurrCapacity >= stackComponent.MaxCapacity)
                    continue;
                
                ref var plantComponent = ref plantPool.Value.Get(plant);
                if (plantComponent.HasFetus==false)
                    continue;
                
                plantComponent.HasFetus = false;
                
                plantComponent.Fetus.Unpack(world, out int fetus);
                serv.Value.HarvestItem(fetus,unit);
                
                
            }
        }
    }
}
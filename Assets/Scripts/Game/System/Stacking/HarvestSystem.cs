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
        private EcsPoolInject<SpeedComponent> speedPool = default;
        private EcsPoolInject<DirectionComponent> directionPool = default;
        private EcsPoolInject<MoveToTargetComponent> moveToPool = default;
        private EcsPoolInject<StackIndexComponent> stackIndexPool = default;
        private EcsPoolInject<AnimatorComponent> animatorPool = default;
        private EcsPoolInject<BaseViewComponent> viewPool = default;
        private EcsPoolInject<HarvestingComponent> harvestingPool = default;

      
        
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
                stackComponent.Entities[stackComponent.CurrCapacity] = world.PackEntity(fetus);
                speedPool.Value.Add(fetus).Value = 10f;
                directionPool.Value.Add(fetus).Value=Vector3.zero;
                moveToPool.Value.Add(fetus).Value = world.PackEntity(unit);
                stackIndexPool.Value.Add(fetus).Value = stackComponent.CurrCapacity;

                stackComponent.CurrCapacity++;
                
                animatorPool.Value.Get(fetus).Value.SetTrigger("Jump");
                
            }
        }
    }
}
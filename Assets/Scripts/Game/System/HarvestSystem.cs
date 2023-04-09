using DefaultNamespace;
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
        
        private readonly EcsPoolInject<HarvestEventComponent> harvestPool = Idents.EVENT_WORLD;
        private EcsPoolInject<StackComponent> stackPool = default;
        private EcsPoolInject<PlantComponent> plantPool = default;
        private EcsPoolInject<SpeedComponent> speedPool = default;
        private EcsPoolInject<DirectionComponent> directionPool = default;
        private EcsPoolInject<MoveToTargetComponent> moveToPool = default;
        private EcsPoolInject<StackIndexComponent> stackIndexPool = default;
        private EcsPoolInject<AnimatorComponent> animatorPool = default;
        private EcsPoolInject<BaseViewComponent> viewPool = default;
      
        
        private EcsFilter filter;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            
            filter = eventWorld.Filter<HarvestEventComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                harvestPool.Value.Get(entity).Sender.Unpack(world, out int plant);
                harvestPool.Value.Get(entity).Collider.Unpack(world, out int unit);
                
                ref var stackComponent = ref stackPool.Value.Get(unit);

                if (stackComponent.CurrCapacity >= stackComponent.MaxCapacity)
                    continue;
                
                ref var plantComponent = ref plantPool.Value.Get(plant);
                if (plantComponent.HasFetus==false)
                    continue;
                
                plantComponent.HasFetus = false;
                
                plantComponent.Fetus.Unpack(world, out int fetus);
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
using DefaultNamespace;
using DefaultNamespace.Game.Component.Actions;
using Game.Component;
using Game.Component.Time;
using Game.Mono;
using Game.Service;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.System
{
    public class PuttingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private EcsPoolInject<TickComponent> tickPool = default;
        private EcsPoolInject<StackComponent> stackPool = default;
        private EcsCustomInject<MovementService> service = default;
        private EcsPoolInject<MoveToTargetComponent> moveToPool = default;
        private EcsPoolInject<StackIndexComponent> stackIndexPool = default;
        private EcsPoolInject<AnimatorComponent> animatorPool = default;
        private EcsPoolInject<BaseViewComponent> viewPool = default;
        private EcsPoolInject<PuttingComponent> putPool = default;

        private EcsFilter filter;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            filter = world.Filter<PlayerTag>().Inc<PuttingComponent>().Inc<TickComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in filter)
            {
               
                putPool.Value.Get(unit).Target.Unpack(world, out int table);

                //no place to put
                ref var targetStackComponent = ref stackPool.Value.Get(table);
                if (targetStackComponent.CurrCapacity >= targetStackComponent.MaxCapacity)
                    continue;
                //no items to put
                ref var unitStackComponent = ref stackPool.Value.Get(unit);
                if (unitStackComponent.CurrCapacity == 0)
                    continue;
               
                ref var tickComponent = ref tickPool.Value.Get(unit);
                
                if (tickComponent.CurrentTime>=tickComponent.FinalTime)
                {
                    unitStackComponent.Entities[unitStackComponent.CurrCapacity - 1].Unpack(world, out int fetus);
                   
                    service.Value.SetSpeed(fetus,10f);
                    service.Value.SetDirection(fetus,Vector3.zero);
                    moveToPool.Value.Add(fetus).Value = world.PackEntity(table);
                    stackIndexPool.Value.Get(fetus).Value = targetStackComponent.CurrCapacity;

                    targetStackComponent.CurrCapacity++;
                    unitStackComponent.CurrCapacity--;

                    animatorPool.Value.Get(fetus).Value.SetTrigger("Jump");
                    tickComponent.CurrentTime = 0;
                }

                tickComponent.CurrentTime+=Time.deltaTime;



            }
        }
    }
}
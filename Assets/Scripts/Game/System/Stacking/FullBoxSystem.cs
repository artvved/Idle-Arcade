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
    public class FullBoxSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private EcsPoolInject<ChangingQueueComponent> changingQueuePool = default;
        private EcsPoolInject<CustomerComponent> customerPool = default;
        private EcsPoolInject<FullBoxComponent> fullBoxPool = default;
        private EcsPoolInject<DirectionComponent> dirPool = default;
        private EcsCustomInject<MovementService> service = default;
        private EcsPoolInject<TickComponent> tickPool = default;
        private EcsPoolInject<AnimatorComponent> animatorPool = default;

        private EcsFilter filter;
        private EcsFilter filterWaitingCustomer;
        private EcsFilter filterTick;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<FullBoxComponent>().Inc<StackComponent>().Inc<AnimatorComponent>().Inc<BoxTag>().Exc<TickComponent>().End();
            filterTick = world.Filter<FullBoxComponent>().Inc<StackComponent>().Inc<AnimatorComponent>().Inc<BoxTag>().Inc<TickComponent>().End();
            filterWaitingCustomer = world.Filter<WaitingForBoxComponent>().Inc<CustomerComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var box in filter)
            {
                var animator = animatorPool.Value.Get(box).Value;
                animator.SetTrigger("Close");
                tickPool.Value.Add(box).FinalTime = animator.runtimeAnimatorController.animationClips[0].length;
            }
            
            foreach (var box in filterTick)
            {
                ref var tickComponent = ref tickPool.Value.Get(box);
                tickComponent.CurrentTime += Time.deltaTime;
                
                if (tickComponent.CurrentTime>=tickComponent.FinalTime)
                {
                    foreach (var cust in filterWaitingCustomer)
                    {
                        service.Value.TranslateItemWithNewIndex(box,cust,0);
                      
                        tickPool.Value.Del(box);
                    }
                   
                }
            }
            
            
           
        }

     
    }
}
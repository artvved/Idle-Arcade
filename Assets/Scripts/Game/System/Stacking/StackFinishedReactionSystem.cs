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
    public class StackFinishedReactionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private EcsPoolInject<ChangingQueueComponent> changingQueuePool = default;
        private EcsPoolInject<CustomerComponent> customerPool = default;
        private EcsPoolInject<FullBoxComponent> fullBoxPool = default;
      
        private EcsPoolInject<StackFinishedComponent> finishedPool = default;
        private EcsPoolInject<WaitingForBoxComponent> waitingPool = default;

        private EcsFilter filter;
        private EcsFilter filterBox;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            filter = world.Filter<StackFinishedComponent>().End();
            filterBox = world.Filter<BoxTag>().Inc<StackComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in filter)
            {
                var taker = finishedPool.Value.Get(unit).IsTaker;

                if (!taker && customerPool.Value.Has(unit))
                {
                    waitingPool.Value.Add(unit);
                    foreach (var box in filterBox)
                    {
                        fullBoxPool.Value.Add(box);
                    }
                   
                }
                else if (taker && customerPool.Value.Has(unit))
                {
                    changingQueuePool.Value.Add(unit);
                }
                
                finishedPool.Value.Del(unit);
            }
        }

     
    }
}
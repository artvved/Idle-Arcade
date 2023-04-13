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
    public class StackingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private EcsPoolInject<TickComponent> tickPool = default;
        private EcsPoolInject<StackComponent> stackPool = default;
        private EcsCustomInject<MovementService> service = default;
        private EcsPoolInject<ItemTranslationComponent> putPool = default;
        private EcsPoolInject<AnimatingTag> animatingPool = default;
        private EcsPoolInject<StackFinishedComponent> finishedPool = default;

        private EcsFilter filter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<ItemTranslationComponent>().Inc<TickComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in filter)
            {
                int inUnit = unit;
                var itemTranslationComponent = putPool.Value.Get(inUnit);
                itemTranslationComponent.Target.Unpack(world, out int target);

                ref var tickComponent = ref tickPool.Value.Get(inUnit);

                var isPutting = itemTranslationComponent.IsPutting;
                if (!isPutting)
                {
                    (target, inUnit) = (inUnit, target);
                }

                //no place to put
                ref var targetStackComponent = ref stackPool.Value.Get(target);
                if (IsTargetFull(targetStackComponent))
                    continue;
                //no items to put
                ref var giverStackComponent = ref stackPool.Value.Get(inUnit);
                if (IsGiverEmpty(giverStackComponent))
                    continue;


                if (tickComponent.CurrentTime >= tickComponent.FinalTime)
                {
                    giverStackComponent.Entities[giverStackComponent.CurrCapacity - 1].Unpack(world, out int item);
                    if (animatingPool.Value.Has(item))
                        continue;

                    service.Value.TranslateItem(item, inUnit,target);
                    
                    if (IsGiverEmpty(giverStackComponent) && isPutting)
                    {
                        finishedPool.Value.Add(inUnit);
                    }
                    else if (IsTargetFull(targetStackComponent) && !isPutting)
                    {
                        finishedPool.Value.Add(target).IsTaker=true;
                    }


                    tickComponent.CurrentTime = 0;
                }

                tickComponent.CurrentTime += Time.deltaTime;
            }
        }

        private bool IsTargetFull(StackComponent component)
        {
            return component.CurrCapacity >= component.MaxCapacity;
        }

        private bool IsGiverEmpty(StackComponent component)
        {
            return component.CurrCapacity == 0;
        }

      
    }
}
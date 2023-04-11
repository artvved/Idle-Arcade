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
        private EcsWorld eWorld;

        private EcsPoolInject<TickComponent> tickPool = default;
        private EcsPoolInject<StackComponent> stackPool = default;
        private EcsCustomInject<MovementService> service = default;
        private EcsPoolInject<MoveToTargetComponent> moveToPool = default;
        private EcsPoolInject<StackIndexComponent> stackIndexPool = default;
        private EcsPoolInject<AnimatorComponent> animatorPool = default;
        private EcsPoolInject<ItemTranslationComponent> putPool = default;
        private EcsPoolInject<AnimatingTag> animatingPool = default;
        private EcsPoolInject<ChangingQueueComponent> changingQueuePool = default;

        //private readonly EcsPoolInject<FullStackEventComponent> eventPool = Idents.EVENT_WORLD;

        private EcsFilter filter;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eWorld = systems.GetWorld(Idents.EVENT_WORLD);

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

                if (!itemTranslationComponent.IsPutting)
                {
                    (target, inUnit) = (inUnit, target);
                }

                //no place to put
                ref var targetStackComponent = ref stackPool.Value.Get(target);
                if (targetStackComponent.CurrCapacity >= targetStackComponent.MaxCapacity)
                    continue;
                //no items to put
                ref var giverStackComponent = ref stackPool.Value.Get(inUnit);
                if (giverStackComponent.CurrCapacity == 0)
                    continue;
                
                
                if (tickComponent.CurrentTime>=tickComponent.FinalTime)
                {
                    giverStackComponent.Entities[giverStackComponent.CurrCapacity - 1].Unpack(world, out int fetus);
                    if (animatingPool.Value.Has(fetus))
                        continue;
                    
                   
                    service.Value.SetSpeed(fetus,10f);
                    service.Value.SetDirection(fetus,Vector3.zero);
                    moveToPool.Value.Add(fetus).Value = world.PackEntity(target);
                    stackIndexPool.Value.Get(fetus).Value = targetStackComponent.CurrCapacity;

                    targetStackComponent.Entities[targetStackComponent.CurrCapacity] = world.PackEntity(fetus);
                    animatingPool.Value.Add(fetus);
                    animatorPool.Value.Get(fetus).Value.SetTrigger("Jump");

                    
                    targetStackComponent.CurrCapacity++;
                    giverStackComponent.CurrCapacity--;
                    tickComponent.CurrentTime = 0;
                    
                    if (targetStackComponent.CurrCapacity==targetStackComponent.MaxCapacity)
                    {
                        changingQueuePool.Value.Add(target);
                    }
                }

                tickComponent.CurrentTime+=Time.deltaTime;
                
            }
        }
        
    }
}
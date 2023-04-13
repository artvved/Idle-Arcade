using DefaultNamespace;
using DefaultNamespace.Game.Component.Actions;
using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.System
{
    public class ReachedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private EcsPoolInject<BaseViewComponent> transformPool = default;
        private EcsPoolInject<DirectionComponent> directionPool = default;
        private EcsPoolInject<SpeedComponent> speedPool = default;
        private EcsPoolInject<MoveToTargetComponent> targetPool = default;
        private EcsPoolInject<StackComponent> stackPool = default;
        private EcsPoolInject<AnimatingTag> animatingPool = default;
        private EcsPoolInject<ReachedTargetComponent> reachedPool = default;
        private EcsPoolInject<BoxTag> boxPool = default;
        private EcsPoolInject<WaitingForBoxComponent> waitingPool = default;
        private EcsPoolInject<ChangingQueueComponent> changingQueuePool = default;
        private EcsPoolInject<DeadTag> deadPool = default;
        private EcsPoolInject<CashTableComponent> cashTablePool= default;
        private EcsPoolInject<CoinTag> coinPool= default;
        private EcsPoolInject<PlayerTag> playerPool= default;
     
        
        private EcsPoolInject<CoinsChangedEventComponent> gainEventPool= Idents.EVENT_WORLD;
        private EcsCustomInject<Fabric> fabric = default;

        private EcsFilter reachedTargetFilter;
        private EcsFilter cashTableFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            
            reachedTargetFilter = world.Filter<ReachedTargetComponent>().Inc<MoveToTargetComponent>().End();
            cashTableFilter = world.Filter<CashTableComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in reachedTargetFilter)
            {
                targetPool.Value.Get(entity).Value.Unpack(world, out int target);

                reachedPool.Value.Del(entity);
                animatingPool.Value.Del(entity);
                targetPool.Value.Del(entity);
                directionPool.Value.Del(entity);
                speedPool.Value.Del(entity);

                if (coinPool.Value.Has(entity) )
                {
                    if (playerPool.Value.Has(target))
                    {
                      
                        gainEventPool.NewEntity(out int e);
                        deadPool.Value.Add(entity);
                    }
                    else
                    {
                        foreach (var table in cashTableFilter)
                        {
                            cashTablePool.Value.Get(table).Money.Unpack(world, out int money);
                            if (money==target)
                            {
                               
                                ref var stackComponent =ref stackPool.Value.Get(money);
                                stackComponent.Entities[stackComponent.CurrCapacity] = world.PackEntity(entity);
                                stackComponent.CurrCapacity++;
                            }
                        }
                    }
                }

                if (boxPool.Value.Has(entity))
                {
                    boxPool.Value.Del(entity);
                    changingQueuePool.Value.Add(target);
                    waitingPool.Value.Del(target);
                    var ecsPackedEntities = stackPool.Value.Get(entity).Entities;
                    foreach (var itemInBox in ecsPackedEntities)
                    {
                        if ( itemInBox.Unpack(world, out int dead))
                        {
                            deadPool.Value.Add(dead);
                        }
                    }

                    foreach (var table in cashTableFilter)
                    {
                        var view = (CashTableView) transformPool.Value.Get(table).Value;                        
                        var newBox = fabric.Value.InstantiateBox(view.BoxSpawnPlace.position);
                        cashTablePool.Value.Get(table).Box = world.PackEntity(newBox);
                    }
                   
                }
            }
        }
    }
}
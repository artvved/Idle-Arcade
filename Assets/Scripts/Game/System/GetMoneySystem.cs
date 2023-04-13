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
    public class GetMoneySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
     
        private readonly EcsPoolInject<CashTableComponent> cashTablePool = default;
        private readonly EcsPoolInject<StackComponent> stackPool = default;
        private readonly EcsPoolInject<AnimatingTag> animPool = default;
        private readonly EcsCustomInject<MovementService> move = default;
        
        private EcsFilter filter;
        private EcsFilter cashTableFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            
            filter = world.Filter<PlayerCashActionComponent>().End();
            cashTableFilter = world.Filter<CashTableComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var player in filter)
            {
                foreach (var table in cashTableFilter)
                {
                    cashTablePool.Value.Get(table).Money.Unpack(world,out int money);
                   
                    
                    var comp = stackPool.Value.Get(money);
                    var stacks = comp.Entities;
                    var currCapacity = comp.CurrCapacity;
                    for (int i = 0; i < currCapacity; i++)
                    {
                        if (!stacks[i].Unpack(world, out int item))
                        {
                            Debug.Log("DEAD");
                            continue;
                        }
                       
                        if (animPool.Value.Has(item))
                        {
                            continue;
                        }
                       
                        move.Value.TranslateItemWithoutTargetStack(item,money,player,0);
                    }
                    
                }
            }
        }
        
      
    }
}
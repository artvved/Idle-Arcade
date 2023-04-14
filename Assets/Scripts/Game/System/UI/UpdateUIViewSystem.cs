using DefaultNamespace;
using Game.Component;
using Game.Service;
using Game.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class UpdateUIViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;
        private readonly EcsCustomInject<MovementService> move = default;
        private readonly EcsPoolInject<UIViewComponent> viewPool = default;
       
        private readonly EcsPoolInject<StackComponent> stackPool = default;
        private readonly EcsPoolInject<QueueComponent> queuePool = default;

      
        private EcsFilter filterCustomer;
        private EcsFilter filterPlayer;
        private EcsFilter filterBuyPlace;
        private EcsFilter filterFetusQueue;
        private EcsFilter filterCashQueue;
        private EcsFilter filterExitQueue;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filterCustomer = world.Filter<UIViewComponent>().Inc<CustomerComponent>().Exc<DeadTag>().End();
            filterPlayer = world.Filter<UIViewComponent>().Inc<PlayerTag>().Exc<DeadTag>().End();
            filterBuyPlace = world.Filter<UIViewComponent>().Inc<BuyPlaceTag>().Exc<DeadTag>().End();
            filterCashQueue = world.Filter<QueueComponent>().Inc<CashTableComponent>().End();
            filterExitQueue = world.Filter<QueueComponent>().Inc<ExitTag>().End();
            filterFetusQueue = world.Filter<QueueComponent>().Inc<FetusTableTag>().End();
            
        }

        public void Run(IEcsSystems systems)
        {
            var camera = sceneData.Value.UICamera;
            ChangeThought(filterCashQueue,staticData.Value.CashSprite);
            ChangeThought(filterExitQueue,staticData.Value.SmileSprite);
            ChangeCapacity(filterFetusQueue);
            
            foreach (var entity in filterCustomer)
            {
                move.Value.MoveUI(entity,camera,staticData.Value.ViewCustomerOffset);
            }
            
            foreach (var entity in filterPlayer)
            {
                PlayerUIView view =  ((PlayerUIView) viewPool.Value.Get(entity).Value);
                var comp = stackPool.Value.Get(entity);
                
                if (comp.CurrCapacity>=comp.MaxCapacity)
                    view.ShowHideMax(true);
                else
                    view.ShowHideMax(false);
                move.Value.MoveUI(entity,camera,staticData.Value.ViewPlayerOffset);
            }

            foreach (var entity in filterBuyPlace)
            {
                move.Value.MoveUI(entity,camera,staticData.Value.ViewPlayerOffset);
            }
            
        }
        

        private void ChangeThought(EcsFilter filter,Sprite sprite)
        {
            foreach (var cash in filter)
            {
                var customers = queuePool.Value.Get(cash).Value;
                foreach (var cust in customers)
                {
                    cust.Unpack(world, out int person);
                    CustomerUIView view =  ((CustomerUIView) viewPool.Value.Get(person).Value);
                    view.SetSingleImage(sprite);
                }
            }
        }
        
        private void ChangeCapacity(EcsFilter filter)
        {
            foreach (var cash in filter)
            {
                var customers = queuePool.Value.Get(cash).Value;
                foreach (var cust in customers)
                {
                    cust.Unpack(world, out int person);
                    CustomerUIView view =  ((CustomerUIView) viewPool.Value.Get(person).Value);
                    var comp = stackPool.Value.Get(person);
                
                    view.SetText(comp.CurrCapacity,comp.MaxCapacity);
                }
            }
        }
        


    }
}
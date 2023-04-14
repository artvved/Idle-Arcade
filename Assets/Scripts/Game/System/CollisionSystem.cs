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
    public class CollisionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<OnTriggerEnterEvent> triggerPool = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<OnTriggerExitEvent> triggerExitPool = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<ItemTranslationComponent> putPool = default;
        private readonly EcsPoolInject<PlantComponent> plantPool = default;
        private readonly EcsPoolInject<PlayerTag> playerPool = default;
        private readonly EcsPoolInject<CustomerComponent> customerPool = default;
        private readonly EcsPoolInject<FetusTableTag> fetusTablePool = default;
        private readonly EcsPoolInject<CashTableComponent> cashTablePool = default;
        private readonly EcsPoolInject<ExitTag> exitPool = default;
        private readonly EcsPoolInject<DeadTag> deadPool = default;
        private readonly EcsPoolInject<CustomerSpawnComponent> spawnerPool = default;
        private readonly EcsPoolInject<ChangingQueueComponent> changePool = default;
        private readonly EcsPoolInject<DirectionComponent> dirPool = default;
        private readonly EcsPoolInject<InteractableCustomerComponent> interPool = default;
        private readonly EcsPoolInject<HarvestingComponent> harvestingPool = default;
        private readonly EcsPoolInject<PlayerCashActionComponent> playerCashPool = default;
        private readonly EcsPoolInject<TickComponent> tickPool = default;
        
        private readonly EcsCustomInject<AnimationService> anim = default;

        private EcsFilter enterFilter;
        private EcsFilter exitFilter;
        private EcsFilter playerCashFilter;
        private EcsFilter interactFilter;
        private EcsFilter spawnerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            enterFilter = eventWorld.Filter<OnTriggerEnterEvent>().End();
            exitFilter = eventWorld.Filter<OnTriggerExitEvent>().End();
            playerCashFilter = world.Filter<PlayerCashActionComponent>().End();
            interactFilter = world.Filter<InteractableCustomerComponent>().End();
            spawnerFilter = world.Filter<CustomerSpawnComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var exitEnt in exitFilter)
            {
                var triggerExitEvent = triggerExitPool.Value.Get(exitEnt);

                var senderView = triggerExitEvent.senderGameObject.gameObject.GetComponent<BaseView>();
                var colliderView = triggerExitEvent.collider.gameObject.GetComponent<BaseView>();

                if (senderView == null)
                    continue;
                if (colliderView == null)
                    continue;

                int sender = senderView.Entity;
                int collider = colliderView.Entity;

                if (plantPool.Value.Has(sender))
                {
                    harvestingPool.Value.Del(collider);
                }

                if (fetusTablePool.Value.Has(sender) || cashTablePool.Value.Has(sender))
                {
                    putPool.Value.Del(collider);
                    tickPool.Value.Del(collider);
                    playerCashPool.Value.Del(collider);
                }
            }

            foreach (var triggerEnt in enterFilter)
            {
                Debug.Log(1);
                var triggerEnterEvent = triggerPool.Value.Get(triggerEnt);
                var senderView = triggerEnterEvent.senderGameObject.gameObject.GetComponent<BaseView>();
                var colliderView = triggerEnterEvent.collider.gameObject.GetComponent<BaseView>();

                if (senderView == null)
                    continue;
                if (colliderView == null)
                    continue;

                int sender = senderView.Entity;
                int collider = colliderView.Entity;

                if (plantPool.Value.Has(sender))
                {
                    AddToHarvesting(sender, collider);
                }

                if (fetusTablePool.Value.Has(sender))
                {
                    if (IsPlayer(collider))
                    {
                        AddPutTickComponent(sender, collider, true);
                    }
                    else if (IsCustomer(collider))
                    {
                        AddPutTickComponent(sender, collider, false);
                       
                    }
                }

                if (cashTablePool.Value.Has(sender))
                {
                    if (IsPlayer(collider))
                    {
                        playerCashPool.Value.Add(collider);
                        foreach (var cust in interactFilter)
                        {
                            CashAction(sender, cust);
                        }
                    }
                    else if (IsCustomer(collider))
                    {
                        interPool.Value.Add(collider);
                        dirPool.Value.Add(collider);
                        foreach (var player in playerCashFilter)
                        {
                            CashAction(sender, collider);
                        }
                    }
                }

                if (exitPool.Value.Has(sender))
                {
                    if (IsCustomer(collider))
                    {
                        deadPool.Value.Add(collider);
                        changePool.Value.Add(collider);
                        foreach (var spawner in spawnerFilter)
                        {
                            ref var customerSpawnComponent = ref spawnerPool.Value.Get(spawner);
                            customerSpawnComponent.CustomersCount--;
                        }
                    }
                }
            }
        }

        private bool IsCustomer(int ent)
        {
           return customerPool.Value.Has(ent);
        }
        private bool IsPlayer(int ent)
        {
            return playerPool.Value.Has(ent);
        }

        private void CashAction(int sender, int collider)
        {
            interPool.Value.Del(collider);
            dirPool.Value.Del(collider);
            cashTablePool.Value.Get(sender).Box.Unpack(world, out int box);
            AddPutTickComponent(box, collider, true);
        }
        private void AddToHarvesting(int sender, int collider)
        {
            if (harvestingPool.Value.Has(collider))
                harvestingPool.Value.Get(collider).Target = world.PackEntity(sender);
            else
                harvestingPool.Value.Add(collider).Target = world.PackEntity(sender);
        }

        private void AddPutTickComponent(int sender, int collider, bool isPutting)
        {
            if (putPool.Value.Has(collider))
                putPool.Value.Get(collider).Target = world.PackEntity(sender);
            else
            {
                ref var component = ref putPool.Value.Add(collider);
                component.Target = world.PackEntity(sender);
                component.IsPutting = isPutting;
                ref var tickComponent = ref tickPool.Value.Add(collider);
                tickComponent.CurrentTime = tickComponent.FinalTime = 0.3f; //to static data?
            }
        }
    }
}
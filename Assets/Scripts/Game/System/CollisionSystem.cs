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
        private readonly EcsPoolInject<PuttingComponent> putPool = default;
        private readonly EcsPoolInject<PlantComponent> plantPool = default;
        private readonly EcsPoolInject<FetusTableTag> tablePool = default;

        private readonly EcsPoolInject<HarvestingComponent> harvestingPool = default;
        private readonly EcsPoolInject<TickComponent> tickPool = default;

        private EcsFilter filter;
        private EcsFilter exitFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filter = eventWorld.Filter<OnTriggerEnterEvent>().End();
            exitFilter = eventWorld.Filter<OnTriggerExitEvent>().End();
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
                
                if (tablePool.Value.Has(sender))
                {
                    putPool.Value.Del(collider);
                    tickPool.Value.Del(collider);
                }
            }

            foreach (var triggerEnt in filter)
            {
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
                    if (harvestingPool.Value.Has(collider))
                        harvestingPool.Value.Get(collider).Target = world.PackEntity(sender);
                    else
                        harvestingPool.Value.Add(collider).Target = world.PackEntity(sender);
                }

                if (tablePool.Value.Has(sender))
                {
                    if (putPool.Value.Has(collider))
                        putPool.Value.Get(collider).Target = world.PackEntity(sender);
                    else
                    {
                        putPool.Value.Add(collider).Target = world.PackEntity(sender);
                        ref var tickComponent = ref tickPool.Value.Add(collider);
                        tickComponent.CurrentTime=tickComponent.FinalTime = 0.5f;  //to static data?
                    }
                }
            }
        }
    }
}
using DefaultNamespace;
using Game.Component;
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
       // private readonly EcsPoolInject<OnTriggerExitEvent> triggerExitPool = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<HarvestEventComponent> harvestPool = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<PlantComponent> plantPool = default;
        private readonly EcsPoolInject<FetusTableTag> tablePool = default;
        
        private EcsFilter filter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            
            filter = eventWorld.Filter<OnTriggerEnterEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var triggerEnt in filter)
            {
                var triggerEnterEvent = triggerPool.Value.Get(triggerEnt);
                var senderView =triggerEnterEvent.senderGameObject.gameObject.GetComponent<BaseView>();
                var colliderView=triggerEnterEvent.collider.gameObject.GetComponent<BaseView>();
                
                if (senderView==null)
                    continue;
                if (colliderView==null)
                    continue;
                
                int sender = senderView.Entity;
                int collider = colliderView.Entity;
                
                if (plantPool.Value.Has(sender))
                {
                    var newEntity = eventWorld.NewEntity();
                    ref var  harvestEventComponent = ref harvestPool.Value.Add(newEntity);
                    harvestEventComponent.Sender=world.PackEntity(sender);
                    harvestEventComponent.Collider=world.PackEntity(collider);
                }
                
                if (tablePool.Value.Has(sender))
                {
                    Debug.Log(1);
                }
                
                
            }
        }
    }
}
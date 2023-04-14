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
    public class CustomerReachedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<CustomerViewComponent> viewPool = default;
        
        private readonly EcsCustomInject<AnimationService> anim = default;

        private EcsFilter filter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filter = world.Filter<CustomerComponent>().Inc<CustomerViewComponent>().End();
      
        }

        public void Run(IEcsSystems systems)
        {
            
            foreach (var ent in filter)
            {
                var agent = viewPool.Value.Get(ent).Value.NavMeshAgent;
                
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                  anim.Value.AnimateStopMove(ent);
                }
                else
                {
                    anim.Value.AnimateMove(ent);
                }
            }
        }

       
    }
}
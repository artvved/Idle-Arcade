using DefaultNamespace.Game.Component.Actions;
using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class LookAtPlayerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
      
        readonly EcsPoolInject<BaseViewComponent> transformPool=default;
        
        readonly EcsPoolInject<DirectionComponent> directionPool = default;

        private EcsFilter unitTransformFilter;
        private EcsFilter playerTransformFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<DirectionComponent>()
                .Inc<InteractableCustomerComponent>()
                .Inc<BaseViewComponent>()
                .End();
            playerTransformFilter = world.Filter<PlayerTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                foreach (var player in playerTransformFilter)
                {
                    var playerTr = transformPool.Value.Get(player).Value.transform;
                    ref var direction =ref directionPool.Value.Get(entity).Value;
                    var transform = transformPool.Value.Get(entity).Value.transform;
                    direction = playerTr.position - transform.position;
                }
                
            }
        }
    }
}
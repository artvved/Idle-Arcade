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
    public class RotationApplySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
      
        readonly EcsPoolInject<BaseViewComponent> transformPool=default;
        
        readonly EcsPoolInject<DirectionComponent> directionPool = default;

        private EcsFilter unitTransformFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<SpeedComponent>()
                .Inc<DirectionComponent>()
                .Inc<BaseViewComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                var direction = directionPool.Value.Get(entity).Value;
                var valueTransform = transformPool.Value.Get(entity).Value.transform;
                
                valueTransform.rotation = Quaternion.LookRotation(direction);
                
            }
        }
    }
}
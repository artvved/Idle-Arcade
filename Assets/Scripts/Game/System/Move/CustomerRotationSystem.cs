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
    public class CustomerRotationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
      
        readonly EcsPoolInject<BaseViewComponent> transformPool=default;
        readonly EcsPoolInject<DirectionComponent> directionPool = default;

        private EcsFilter unitTransformFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<DirectionComponent>()
                .Inc<BaseViewComponent>()
                .Inc<CustomerComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                var direction = directionPool.Value.Get(entity).Value;
                var valueTransform = transformPool.Value.Get(entity).Value.transform;
                
                var goal= Quaternion.LookRotation(direction);
                valueTransform.rotation = Quaternion.Slerp(valueTransform.rotation, goal, 0.05f);

            }
        }
    }
}
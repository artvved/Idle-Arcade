using Game.Component;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.System
{
    public class MoveToStackSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        readonly EcsPoolInject<BaseViewComponent> transformPool = default;
        readonly EcsPoolInject<DirectionComponent> directionPool = default;
        readonly EcsPoolInject<SpeedComponent> speedPool = default;
        readonly EcsPoolInject<MoveToTargetComponent> targetPool = default;
        readonly EcsPoolInject<StackIndexComponent> stackIndexPool = default;
        readonly EcsPoolInject<StackComponent> stackPool = default;
        private EcsPoolInject<AnimatingTag> animatingPool = default;
        private EcsFilter unitTransformFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<MoveToTargetComponent>()
                .Inc<SpeedComponent>()
                .Inc<BaseViewComponent>()
                .Inc<DirectionComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                targetPool.Value.Get(entity).Value.Unpack(world, out int target);
                var stackComp = stackPool.Value.Get(target);
                
                var entTrans = transformPool.Value.Get(entity).Value.transform;
              
                var index = stackIndexPool.Value.Get(entity).Value;

                var stackPlace = stackComp.Places[index];
                Vector3 range = stackPlace.position - entTrans.position;

                var dir = range.normalized;
                directionPool.Value.Get(entity).Value = dir;
                
                
                //reached
                if (range.magnitude<0.05f)
                {
                    animatingPool.Value.Del(entity);
                    targetPool.Value.Del(entity);
                    directionPool.Value.Del(entity);
                    speedPool.Value.Del(entity);
                    entTrans.parent= stackPlace;
                }
                
             
               
            }
        }
    }
}
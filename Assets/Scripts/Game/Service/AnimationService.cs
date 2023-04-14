using System.Collections.Generic;
using Game.Component;
using Leopotam.EcsLite;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Game.Service
{
    public class AnimationService {

        private EcsWorld world;
        private EcsPool<BaseViewComponent> baseTransformPool;
        private EcsPool<SpeedComponent> speedPool;
        private EcsPool<DirectionComponent> dirPool;
        
        private EcsPool<MoveToTargetComponent> moveToPool ;
        private EcsPool<StackIndexComponent> stackIndexPool ;
        private EcsPool<AnimatorComponent> animatorPool;
        private EcsPool<AnimatingTag> animatingPool; 
        private EcsPool<StackComponent> stackPool; 
      

        public AnimationService(EcsWorld world)
        {
            this.world = world;
            baseTransformPool = world.GetPool<BaseViewComponent>();
            speedPool = world.GetPool<SpeedComponent>();
            dirPool = world.GetPool<DirectionComponent>();
            moveToPool = world.GetPool<MoveToTargetComponent>();
            stackIndexPool = world.GetPool<StackIndexComponent>();
            animatorPool = world.GetPool<AnimatorComponent>();
            animatingPool = world.GetPool<AnimatingTag>();
            stackPool = this.world.GetPool<StackComponent>();
        }

        public void AnimateMove(int ent,float curVel,float maxVelocity)
        {
            var animator = animatorPool.Get(ent).Value;
            animator.SetBool("Move",true);
            animator.speed = curVel / (float) maxVelocity;
        }
        
        public void AnimateMove(int ent)
        {
            var animator = animatorPool.Get(ent).Value;
            animator.SetBool("Move",true);
        }
        public void AnimateStopMove(int ent)
        {
            var animator = animatorPool.Get(ent).Value;
            animator.SetBool("Move",false);
        }

        public void AnimateCashPanel(int ent,bool activate)
        {
            var animator = animatorPool.Get(ent).Value;
            animator.SetBool("Activate",activate);
           

        }
        
    }
}
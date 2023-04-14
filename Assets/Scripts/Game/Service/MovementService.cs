using System.Collections.Generic;
using Game.Component;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Service
{
    public class MovementService
    {
        private EcsWorld world;
        private EcsPool<BaseViewComponent> baseTransformPool;
        private EcsPool<SpeedComponent> speedPool;
        private EcsPool<DirectionComponent> dirPool;

        private EcsPool<MoveToTargetComponent> moveToPool;
        private EcsPool<StackIndexComponent> stackIndexPool;
        private EcsPool<AnimatorComponent> animatorPool;
        private EcsPool<AnimatingTag> animatingPool;
        private EcsPool<StackComponent> stackPool;
        private EcsPool<UIViewComponent> viewUIPool;


        public MovementService(EcsWorld world)
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
            viewUIPool = this.world.GetPool<UIViewComponent>();
        }

        public void SetSpeed(int ent, float speed)
        {
            speedPool.Add(ent).Value = speed;
        }

        public void SetDirection(int ent, Vector3 dir)
        {
            dirPool.Add(ent).Value = dir;
        }


        public void TranslateItem(int item, int giver, int target)
        {
            ref var giverStack = ref stackPool.Get(giver);
            ref var targetStack = ref stackPool.Get(target);
            TranslateItemWithoutCapacity(item, target, targetStack.CurrCapacity);
            targetStack.Entities[targetStack.CurrCapacity] = world.PackEntity(item);
            targetStack.CurrCapacity++;
            giverStack.CurrCapacity--;
        }

        public void TranslateItemWithoutGiverStack(int item, int target, int index)
        {
            TranslateItemWithoutCapacity(item, target, index);
        }

        public void HarvestItem(int item, int target)
        {
            ref var targetStack = ref stackPool.Get(target);
            TranslateItemWithoutCapacity(item, target, targetStack.CurrCapacity);
            targetStack.Entities[targetStack.CurrCapacity] = world.PackEntity(item);
            targetStack.CurrCapacity++;
        }

        public void TranslateItemWithoutTargetStack(int item, int giver, int target, int index)
        {
            ref var giverStack = ref stackPool.Get(giver);
            TranslateItemWithoutCapacity(item, target, index);
            giverStack.CurrCapacity--;
        }


        public void TranslateItemWithoutCapacity(int item, int target, int index)
        {
            baseTransformPool.Get(item).Value.transform.parent = null;
            SetSpeed(item, 10f);
            SetDirection(item, Vector3.zero);
            moveToPool.Add(item).Value = world.PackEntity(target);
            animatingPool.Add(item);
            animatorPool.Get(item).Value.SetTrigger("Jump");

            if (stackIndexPool.Has(item))
                stackIndexPool.Get(item).Value = index;
            else
                stackIndexPool.Add(item).Value = index;
        }

        public void MoveUI(int entity, Camera camera, Vector2 offset)
        {
            ref var view = ref viewUIPool.Get(entity).Value;
            var pos = baseTransformPool.Get(entity).Value.transform.position;

            var screenPoint = camera.WorldToScreenPoint(pos + (Vector3)offset);
            view.transform.position = screenPoint;
        }
    }
}
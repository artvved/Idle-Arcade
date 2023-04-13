using System.Collections.Generic;
using Game.Component;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Service
{
    public class MovementService {

        private EcsWorld world;
        private EcsPool<BaseViewComponent> baseTransformPool;
        private EcsPool<SpeedComponent> speedPool;
        private EcsPool<DirectionComponent> dirPool;
        
        private EcsPool<MoveToTargetComponent> moveToPool ;
        private EcsPool<StackIndexComponent> stackIndexPool ;
        private EcsPool<AnimatorComponent> animatorPool;
        private EcsPool<AnimatingTag> animatingPool; 
        private EcsPool<StackComponent> stackPool; 
      

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
        }

        public void SetSpeed(int ent,float speed)
        {
            speedPool.Add(ent).Value = speed;
        }
        public void SetDirection(int ent,Vector3 dir)
        {
            dirPool.Add(ent).Value = dir;
        }

        public int GetClosestTarget(int entity,EcsFilter filter)
        {
            var entPos = baseTransformPool.Get(entity).Value.transform.position;
            int closest = -1;
            float range = -1;
            foreach (var target in filter)
            {
                var allyPos = baseTransformPool.Get(target).Value.transform.position;
                if (closest == -1 || (entPos - allyPos).magnitude < range)
                {
                    closest = target;
                    range = (entPos - allyPos).magnitude;
                }
            }
            
            return closest;
        }
        
        public void TranslateItem(int item, int giver,int target)
        {
            ref var giverStack = ref stackPool.Get(giver);
            ref var targetStack = ref stackPool.Get(target);
            TranslateItemWithoutCapacity(item, target, targetStack.CurrCapacity);
            targetStack.Entities[targetStack.CurrCapacity] = world.PackEntity(item);
            targetStack.CurrCapacity++;
            giverStack.CurrCapacity--;
        }
        
        public void TranslateItemWithoutGiverStack(int item, int target,int index)
        {
            TranslateItemWithoutCapacity(item, target, index);
        }
        public void TranslateItemWithoutTargetStack(int item, int giver,int target,int index)
        {
            ref var giverStack = ref stackPool.Get(giver);
            TranslateItemWithoutCapacity(item, target, index);
            giverStack.CurrCapacity--;
        }
      
        
        public void TranslateItemWithoutCapacity(int item, int target,int index)
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
        
        
        
        public int GetRandomTarget(EcsFilter filter)
        {
            List<int> list = new List<int>();
            foreach (var target in filter)
            {
                list.Add(target);
            }

            var rnd = Random.Range(0, list.Count);
            if (list.Count>0)
            {
                return list[rnd];
            }
            else
            {
                return -1;
            }
           
        }
        
        public List<int> GetTargetsInRange(int entity,EcsFilter filter,float range)
        {
            List<int> list = new List<int>();
            var entPos = baseTransformPool.Get(entity).Value.transform.position;
            
            foreach (var target in filter)
            {
                var pos2 = baseTransformPool.Get(target).Value.transform.position;
               
                if (IsInRange(entPos,pos2,range))
                {
                    list.Add(target);
                }
            }
            
            return list;
        }

        public int GetClosestTargetWithRange(int entity, EcsFilter filter,float range)
        {
            var closestTarget = GetClosestTarget(entity, filter);
            var pos1 = baseTransformPool.Get(entity).Value.transform.position;
            var pos2 = baseTransformPool.Get(closestTarget).Value.transform.position;
            
            if (!IsInRange(pos1,pos2,range))
            {
                return -1;
            }

            return closestTarget;
        }

        public bool IsInRange(Vector3 pos1, Vector3 pos2, float range)
        {
            return (pos1 - pos2).magnitude <= range;
        }
        

        public float GetRange(int ent1,int ent2)
        {
            var entPos1 = baseTransformPool.Get(ent1).Value.transform.position;
            var entPos2 = baseTransformPool.Get(ent2).Value.transform.position;
            return (entPos1 - entPos2).magnitude;
        }
        
        public Vector3 GetDirection(int ent1,int ent2)
        {
            var entPos1 = baseTransformPool.Get(ent1).Value.transform.position;
            var entPos2 = baseTransformPool.Get(ent2).Value.transform.position;
            return (entPos2 - entPos1).normalized;
        }
    }
}
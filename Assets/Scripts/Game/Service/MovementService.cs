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

        public MovementService(EcsWorld world)
        {
            this.world = world;
            baseTransformPool = world.GetPool<BaseViewComponent>();
            speedPool = world.GetPool<SpeedComponent>();
            dirPool = world.GetPool<DirectionComponent>();
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
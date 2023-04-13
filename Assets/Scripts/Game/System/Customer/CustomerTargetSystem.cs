using System.Collections.Generic;
using DefaultNamespace;
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
    public class CustomerTargetSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        readonly EcsPoolInject<CustomerViewComponent> viewPool = default;
        readonly EcsPoolInject<QueueComponent> queuePool = default;
        readonly EcsPoolInject<InQueueComponent> inQueuePool = default;
        readonly EcsPoolInject<ChangingQueueComponent> changePool = default;
        //queue targets
        readonly EcsPoolInject<FetusTableTag> tablePool = default;
        readonly EcsPoolInject<CashTableComponent> cashPool = default;
        
        private EcsFilter queueFilter;
        
        private EcsFilter queueCashFilter;
        private EcsFilter queueExitFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            queueFilter = world.Filter<QueueComponent>().End();
            queueCashFilter = world.Filter<QueueComponent>().Inc<CashTableComponent>().End();
            queueExitFilter = world.Filter<QueueComponent>().Inc<ExitTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in queueFilter)
            {
                var queueComponent = queuePool.Value.Get(ent);
                var queue = queueComponent.Value;

                int first = -1;
                if (!TryGetFirst(ref first, queue))
                    continue;
                
                if (changePool.Value.Has(first))
                {
                    UpdateQueue(queue);
                    ChangeQueue(first,queue,ent);
                }
            
                if (!TryGetFirst(ref first, queue))
                    continue;

                //1st goes to target
                if (!inQueuePool.Value.Has(first))
                {
                    var firstView = viewPool.Value.Get(first).Value;
                    firstView.NavMeshAgent.destination = queueComponent.Target.position;
                    firstView.NavMeshAgent.stoppingDistance = 0;
                    inQueuePool.Value.Add(first);
                }
              
                
                //others make queue
                for (int i = queue.Count-1; i >= 1; i--)
                {
                    queue[i].Unpack(world, out int entity);
                    if (inQueuePool.Value.Has(entity))
                        continue;

                    var customerView = viewPool.Value.Get(entity).Value;
                    queue[i-1].Unpack(world, out int target);

                    customerView.NavMeshAgent.destination = viewPool.Value.Get(target).Value.transform.position;
                    customerView.NavMeshAgent.stoppingDistance = 3;
                    inQueuePool.Value.Add(entity);
                }
                
            }

        }

        private bool TryGetFirst(ref int res, List<EcsPackedEntity> queue)
        {
            if (queue.Count!=0 && queue[0].Unpack(world, out int first))
            {
                res = first;
                return true;
            }
            return false;
            
        }

        private void ChangeQueue(int first, List<EcsPackedEntity> queue,int queueEnt)
        {
            //add to other queue
            if (tablePool.Value.Has(queueEnt))
            {
                foreach (var cashEnt in queueCashFilter)
                {
                    queuePool.Value.Get(cashEnt).Value.Add(world.PackEntity(first));
                }
            }
            
            if (cashPool.Value.Has(queueEnt))
            {
                foreach (var exitEnt in queueExitFilter)
                {
                    queuePool.Value.Get(exitEnt).Value.Add(world.PackEntity(first));
                }
            }
           
            changePool.Value.Del(first);
            queue.RemoveAt(0);
            
        }

        private void UpdateQueue(List<EcsPackedEntity> queue)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                queue[i].Unpack(world, out int entity);
                inQueuePool.Value.Del(entity);
            }
        }
    }
}
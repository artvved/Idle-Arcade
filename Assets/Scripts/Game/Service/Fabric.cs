using System.Collections.Generic;

using Game.Component;
using Game.Component.Time;
using Game.Mono;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Game.Service
{
    public class Fabric
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private StaticData staticData;


        private EcsPool<PlayerTag> playerPool;
        private EcsPool<CustomerComponent> customerPool;
        private EcsPool<FetusTag> fetusPool;
        private EcsPool<FetusTableTag> fetusTablePool;
        private EcsPool<CashTableTag> cashTablePool;
        private EcsPool<UnitStatsComponent> unitStatsPool;
        private EcsPool<PlantComponent> plantPool;
        private EcsPool<QueueComponent> queuePool;
       

        private EcsPool<BaseViewComponent> baseViewPool;
        private EcsPool<UnitViewComponent> unitViewPool;
        private EcsPool<PlantViewComponent> plantViewPool;
        private EcsPool<CustomerViewComponent> customerViewPool;

        private EcsPool<AnimatingTag> animatingPool;
        private EcsPool<AnimatorComponent> animatorPool;
        private EcsPool<TickComponent> tickPool;
        private EcsPool<StackComponent> stackPool;

        public Fabric(EcsWorld world, EcsWorld eventWorld, StaticData staticData)
        {
            this.world = world;
            this.eventWorld = eventWorld;
            this.staticData = staticData;


            playerPool = world.GetPool<PlayerTag>();
            customerPool = world.GetPool<CustomerComponent>();
            unitStatsPool = world.GetPool<UnitStatsComponent>();
            queuePool = this.world.GetPool<QueueComponent>();

            baseViewPool = world.GetPool<BaseViewComponent>();
            animatorPool = this.world.GetPool<AnimatorComponent>();
            plantViewPool = this.world.GetPool<PlantViewComponent>();
            customerViewPool = this.world.GetPool<CustomerViewComponent>();
            
            animatingPool = this.world.GetPool<AnimatingTag>();
            tickPool = this.world.GetPool<TickComponent>();
            plantPool = world.GetPool<PlantComponent>();
            stackPool = this.world.GetPool<StackComponent>();
            unitViewPool = this.world.GetPool<UnitViewComponent>();
            fetusTablePool = this.world.GetPool<FetusTableTag>();
            fetusPool = world.GetPool<FetusTag>();
            
            cashTablePool = this.world.GetPool<CashTableTag>();
        }


        private int InstantiateObj(BaseView prefab, Vector3 position)
        {
            var view = GameObject.Instantiate(prefab);
            view.transform.position = position;
            int unitEntity = world.NewEntity();
            view.Entity = unitEntity;

            baseViewPool.Add(unitEntity).Value = view;
            animatorPool.Add(unitEntity).Value = view.GetComponent<Animator>();

            return unitEntity;
        }

        public int InstantiatePlant(Transform root)
        {
            var en = InstantiateObj(staticData.PlantPrefab, root.position);
            var plantView = (PlantView) baseViewPool.Get(en).Value;
            plantView.transform.parent = root;
            plantViewPool.Add(en).Value = plantView;
            tickPool.Add(en).FinalTime = staticData.PlantTime;
            plantPool.Add(en);
            return en;
        }

        public int InstantiatePlayer()
        {
            var playerEntity = InstantiateObj(staticData.PlayerPrefab, Vector3.zero);
            playerPool.Add(playerEntity);
            ref var playerStatsComponent = ref unitStatsPool.Add(playerEntity);
            var unitStats = staticData.PlayerStats;
            var stackData = unitStats.StackData;
            playerStatsComponent.Coins = unitStats.Coins;
            playerStatsComponent.MaxSpeed = unitStats.MaxSpeed;

            var unitView = (UnitView) baseViewPool.Get(playerEntity).Value;

            InitStacks(stackData, unitView, playerEntity);
            unitViewPool.Add(playerEntity).Value = unitView;

            return playerEntity;
        }
        
        public int InstantiateCustomer(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.CustomerPrefab, pos);
            customerPool.Add(entity);
            

            var unitStats = staticData.CustomerStats;
            var stackData = unitStats.StackData;
            
            ref var statsComponent = ref unitStatsPool.Add(entity);
            statsComponent.Coins = unitStats.Coins;
            statsComponent.MaxSpeed = unitStats.MaxSpeed;
            
            var unitView = (UnitView) baseViewPool.Get(entity).Value;

            InitStacks(stackData, unitView, entity);
            unitViewPool.Add(entity).Value = unitView;
            ((CustomerView) unitView).NavMeshAgent.speed = unitStats.MaxSpeed;
            customerViewPool.Add(entity).Value = (CustomerView) unitView;

            return entity;
        }


        public int InstantiateTable(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.FetusTablePrefab, pos);
            fetusTablePool.Add(entity);

            var unitView = (UnitView) baseViewPool.Get(entity).Value;
            
            ref var queueComponent = ref queuePool.Add(entity);
            queueComponent.Value = new List<EcsPackedEntity>();
            queueComponent.Target = unitView.transform;

            InitStacks(staticData.FetusTableStackData, unitView, entity);
            unitViewPool.Add(entity).Value = unitView;

            return entity;
        }

        private void InitStacks(StackData stackData, UnitView view, int entity)
        {
            ref var stackComponent = ref stackPool.Add(entity);
            var maxCapacity = stackData.XRows * stackData.YRows * stackData.ZRows;
            stackComponent.MaxCapacity = maxCapacity;
            stackComponent.Places = new Transform[maxCapacity];
            stackComponent.Entities = new EcsPackedEntity[maxCapacity];
            int count = 0;
            for (int y = 0; y < stackData.YRows; y++)
            {
                for (int x = 0; x < stackData.XRows; x++)
                {
                    for (int z = 0; z < stackData.ZRows; z++)
                    {
                        var go = new GameObject();
                        go.transform.position = view.StackPlace.position + new Vector3(x, y, z) * stackData.Offset;
                        go.transform.parent = view.StackPlace;
                        stackComponent.Places[count] = go.transform;
                        count++;
                    }
                }
            }
        }
        
        public int InstantiateFetus(Transform fetusPlace)
        {
            var ent = InstantiateObj(staticData.FetusPrefab, fetusPlace.position);
            fetusPool.Add(ent);
            //animatingPool.Add(ent);
            return ent;
        }
    }
}
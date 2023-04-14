using System.Collections.Generic;

using Game.Component;
using Game.Component.Time;
using Game.Mono;
using Game.UI;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;

namespace Game.Service
{
    public class Fabric
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private StaticData staticData;
        private SceneData sceneData;
        
        private EcsPool<PlayerTag> playerPool;
        private EcsPool<CustomerComponent> customerPool;
        private EcsPool<FetusTag> fetusPool;
        private EcsPool<BoxTag> boxPool;
        private EcsPool<CoinTag> coinPool;
        private EcsPool<UIViewComponent> uiPool;

        private EcsPool<FetusTableTag> fetusTablePool;
        private EcsPool<CashTableComponent> cashTablePool;
        private EcsPool<ExitTag> exitPool;
      
        private EcsPool<UnitStatsComponent> unitStatsPool;
        private EcsPool<PlantComponent> plantPool;
        private EcsPool<QueueComponent> queuePool;
        private EcsPool<NoRotationComponent> noRotPool;

        private EcsPool<BaseViewComponent> baseViewPool;
        private EcsPool<PlantViewComponent> plantViewPool;
        private EcsPool<CustomerViewComponent> customerViewPool;

        private EcsPool<AnimatingTag> animatingPool;
        private EcsPool<AnimatorComponent> animatorPool;
        private EcsPool<TickComponent> tickPool;
        private EcsPool<StackComponent> stackPool;

        public Fabric(EcsWorld world, EcsWorld eventWorld, StaticData staticData,SceneData sceneData)
        {
            this.world = world;
            this.eventWorld = eventWorld;
            this.staticData = staticData;
            this.sceneData = sceneData;

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
         
            fetusTablePool = this.world.GetPool<FetusTableTag>();
            fetusPool = world.GetPool<FetusTag>();
            
            cashTablePool = this.world.GetPool<CashTableComponent>();
            exitPool = this.world.GetPool<ExitTag>();
            boxPool = this.world.GetPool<BoxTag>();
            noRotPool = this.world.GetPool<NoRotationComponent>();
            coinPool = this.world.GetPool<CoinTag>();
            uiPool = this.world.GetPool<UIViewComponent>();
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
            var data = unitStats.StackData;
            playerStatsComponent.Coins = unitStats.Coins;
            playerStatsComponent.MaxSpeed = unitStats.MaxSpeed;

            var unitView = (UnitView) baseViewPool.Get(playerEntity).Value;
            InitStacks(data.XRows,data.YRows,data.ZRows,data.Offset, unitView.StackPlace, playerEntity);
            
            PlayerUIView view= GameObject.Instantiate(staticData.PlayerUIViewPrefab, sceneData.GameUICanvas.transform);
            uiPool.Add(playerEntity).Value = view;
         

            return playerEntity;
        }
        
        public int InstantiateCustomer(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.CustomerPrefab, pos);
            customerPool.Add(entity);
            
            var unitStats = staticData.CustomerStats;
            var data = unitStats.StackData;
            
            var unitView = (UnitView) baseViewPool.Get(entity).Value;
            var rnd = Random.Range(0, 3);
            
            InitStacks(data.XRows,data.YRows+rnd,data.ZRows,data.Offset, unitView.StackPlace, entity);
          
            ((CustomerView) unitView).NavMeshAgent.speed = unitStats.MaxSpeed;
            customerViewPool.Add(entity).Value = (CustomerView) unitView;
            CapacityView view= GameObject.Instantiate(staticData.CapacityViewPrefab, sceneData.GameUICanvas.transform);
            uiPool.Add(entity).Value = view;
            view.SetText(0,data.XRows*(data.YRows+rnd)*data.ZRows);
            
            ref var statsComponent = ref unitStatsPool.Add(entity);
            statsComponent.Coins = unitStats.Coins +rnd*2;
            statsComponent.MaxSpeed = unitStats.MaxSpeed;

            return entity;
        }


        public int InstantiateTable(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.FetusTablePrefab, pos);
            fetusTablePool.Add(entity);

            var unitView = (FetusTableView) baseViewPool.Get(entity).Value;
            
            ref var queueComponent = ref queuePool.Add(entity);
            queueComponent.Value = new List<EcsPackedEntity>();
            queueComponent.Target = unitView.CustomerTarget;

            var data = staticData.FetusTableStackData;
            InitStacks(data.XRows,data.YRows,data.ZRows,data.Offset, unitView.StackPlace, entity);

            return entity;
        }
        
        public int InstantiateCashTable(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.CashTablePrefab, pos);
           
            var view = (CashTableView) baseViewPool.Get(entity).Value;
            
            ref var queueComponent = ref queuePool.Add(entity);
            queueComponent.Value = new List<EcsPackedEntity>();
            queueComponent.Target = view.CustomerTarget;

            int box=InstantiateBox(view.BoxSpawnPlace.position);
            ref var cashTableComponent = ref cashTablePool.Add(entity);
            cashTableComponent.Box=world.PackEntity(box);

            var moneyPlace = InstantiateMoneyPlace(view.MoneyPlace.position);
            cashTableComponent.Money = world.PackEntity(moneyPlace);
            
            return entity;
        }
        
        public int InstantiateExit(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.ExitPrefab, pos);
           
            var view = (SpecialPlaceView) baseViewPool.Get(entity).Value;
            
            ref var queueComponent = ref queuePool.Add(entity);
            queueComponent.Value = new List<EcsPackedEntity>();
            queueComponent.Target = view.CustomerTarget;

            exitPool.Add(entity);

            return entity;
        }
        
        public int InstantiateBox(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.BoxPrefab, pos);
            boxPool.Add(entity);
            var view = (UnitView)baseViewPool.Get(entity).Value;
            var data = staticData.BoxStackData;
            InitStacks(data.XRows,data.YRows,data.ZRows,data.Offset, view.StackPlace, entity);
            return entity;
        }
        
        public int InstantiateMoneyPlace(Vector3 pos)
        {
            var entity = InstantiateObj(staticData.MoneyPlacePrefab, pos);
            var view = (UnitView)baseViewPool.Get(entity).Value;
            var data = staticData.MoneyStackData;
            InitStacks(data.XRows,data.YRows,data.ZRows,data.Offset, view.StackPlace, entity);
            return entity;
        }

        private void InitStacks(int xRows,int yRows,int zRows,float offset, Transform stackPlace, int entity)
        {
            ref var stackComponent = ref stackPool.Add(entity);
            var maxCapacity = xRows * yRows * zRows;
            stackComponent.MaxCapacity = maxCapacity;
            stackComponent.Places = new Transform[maxCapacity];
            stackComponent.Entities = new EcsPackedEntity[maxCapacity];
            int count = 0;
            for (int y = 0; y < yRows; y++)
            {
                for (int x = 0; x < xRows; x++)
                {
                    for (int z = 0; z < zRows; z++)
                    {
                        var go = new GameObject();
                        go.transform.position = stackPlace.position + new Vector3(x, y, z) * offset;
                        go.transform.parent = stackPlace;
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
            animatorPool.Get(ent).Value.applyRootMotion = false;
            noRotPool.Add(ent);
            return ent;
        }
        
        public int InstantiateCoin(Vector3 pos)
        {
            var ent = InstantiateObj(staticData.CoinPrefab, pos);
            coinPool.Add(ent);
            animatorPool.Get(ent).Value.applyRootMotion = false;
            noRotPool.Add(ent);
            return ent;
        }
    }
}
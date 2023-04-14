using DefaultNamespace;
using Game.Component;
using Game.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class UpdateCapacityViewSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private EcsWorld world;

        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<UIViewComponent> viewPool = default;
        private readonly EcsPoolInject<BaseViewComponent> transformPool = default;
        private readonly EcsPoolInject<StackComponent> stackPool = default;

        private EcsFilter filter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<UIViewComponent>().Inc<CustomerComponent>().Exc<DeadTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                CapacityView view =  ((CapacityView) viewPool.Value.Get(entity).Value);

                var comp = stackPool.Value.Get(entity);
                view.SetText(comp.CurrCapacity,comp.MaxCapacity);
            }
        }


        public void PostRun(IEcsSystems systems)
        {
            var camera = sceneData.Value.UICamera;
            foreach (var entity in filter)
            {
                ref var view = ref viewPool.Value.Get(entity).Value;
                var pos = transformPool.Value.Get(entity).Value.transform.position;

                var screenPoint = camera.WorldToScreenPoint(pos);
                view.transform.position = screenPoint+new Vector3(0,staticData.Value.ViewYOffset);
            }
        }
    }
}
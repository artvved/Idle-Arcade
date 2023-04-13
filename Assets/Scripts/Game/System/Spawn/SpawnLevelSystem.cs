using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class SpawnLevelSystem : IEcsInitSystem
    {
        readonly EcsCustomInject<Fabric> fabric=default;
        readonly EcsCustomInject<SceneData> sceneData = default;
        

        public void Init(IEcsSystems systems)
        {
            fabric.Value.InstantiateTable(sceneData.Value.FetusTablePlace.position);
            fabric.Value.InstantiateCashTable(sceneData.Value.CashTablePlace.position);
            fabric.Value.InstantiateExit(sceneData.Value.CustomerExitPlace.position);
            
            var places = sceneData.Value.PlantPlaces;
            foreach (var place in places)
            {
                var entity=fabric.Value.InstantiatePlant(place);
            }
        }


       
    }
}
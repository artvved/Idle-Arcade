using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class SpawnPlantsSystem : IEcsInitSystem
    {
        readonly EcsCustomInject<Fabric> fabric=default;
        readonly EcsCustomInject<SceneData> sceneData = default;
        

        public void Init(IEcsSystems systems)
        {
            var places = sceneData.Value.PlantPlaces;

            foreach (var place in places)
            {
                var entity=fabric.Value.InstantiatePlant(place);
            }
            
        }


       
    }
}
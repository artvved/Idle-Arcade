using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class SpawnTableSystem : IEcsInitSystem
    {
        readonly EcsCustomInject<Fabric> fabric=default;
        readonly EcsCustomInject<SceneData> sceneData = default;
        

        public void Init(IEcsSystems systems)
        {
            fabric.Value.InstantiateFetusTable(sceneData.Value.FetusTablePlace.position);
        }


       
    }
}
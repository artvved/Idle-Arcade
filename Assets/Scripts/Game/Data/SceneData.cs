using Cinemachine;
using Game.Service;
using Game.UI;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;

namespace ScriptableData
{
    public class SceneData : MonoBehaviour
    {
        public EcsUguiEmitter EcsUguiEmitter;
        public CinemachineVirtualCamera Camera;
       
        public CoinsView CoinsView;
        public FloatingJoystick Joystick;

        [Header("Level")]
        public Transform[] PlantPlaces;
        public Transform FetusTablePlace;
        public Transform CashTablePlace;
        public Transform CustomerSpawnPlace;
        public Transform CustomerExitPlace;

    }
}
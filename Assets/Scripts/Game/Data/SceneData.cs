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
        public Camera UICamera;
       
        public CoinsView CoinsView;
        public FloatingJoystick Joystick;
        public Canvas GameUICanvas;

        [Header("Level")]
        public Transform[] PlantPlaces;
        public Transform FetusTablePlace;
        public Transform CashTablePlace;
        public Transform CustomerSpawnPlace;
        public Transform CustomerExitPlace;
        public Transform BuyPlace1;
        public Transform BuyPlace2;

    }
}
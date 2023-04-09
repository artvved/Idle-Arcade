using Game.Mono;
using Game.UI;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        public UnitStats PlayerStats;
        public StackData FetusTableStackData;
        
        public PlayerView PlayerPrefab;
        public PlantView PlantPrefab;
        public BaseView FetusPrefab;
        public FetusTableView FetusTablePrefab;

        public float PlantTime;

        
        //ui


    }
}
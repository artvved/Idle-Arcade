using Game.Mono;
using Game.UI;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        public UnitStats PlayerStats;
        public UnitStats CustomerStats;
        public StackData FetusTableStackData;
        public StackData BoxStackData;
        public StackData MoneyStackData;

        public PlayerView PlayerPrefab;
        public CustomerView CustomerPrefab;
        public PlantView PlantPrefab;
        public BaseView FetusPrefab;
        public FetusTableView FetusTablePrefab;
        public CashTableView CashTablePrefab;
        public ExitView ExitPrefab;
        public BoxView BoxPrefab;
        public MoneyPlaceView MoneyPlacePrefab;
        public BaseView CoinPrefab;
        

        public float PlantTime;
        public float CustomerTime;

        public int MaxCustomers;
        
        //ui
        public CapacityView CapacityViewPrefab;
        public PlayerUIView PlayerUIViewPrefab;
        public float ViewYOffset;

    }
}
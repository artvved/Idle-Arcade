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
        public BuyPlaceView BuyPlacePrefab;
        public BuyPlaceUiView BuyPlaceUiPrefab;
        

        public float PlantTime;
        public float CustomerTime;

        public int MaxCustomers;
        
        //ui
        [Header("UI")]
        public CustomerUIView CustomerUIViewPrefab;
        public PlayerUIView PlayerUIViewPrefab;
        public Vector2 ViewCustomerOffset;
        public Vector2 ViewPlayerOffset;
        public Sprite CashSprite;
        public Sprite SmileSprite;


    }
}
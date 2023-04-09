using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class UnitStats : ScriptableObject
    {
        public float MaxSpeed;
        public int Coins;
        public StackData StackData;

    }
}
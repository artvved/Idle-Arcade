using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StackData : ScriptableObject
    {
        public int MaxCapacity;
        public float Offset;
        public int XRows;
        public int YRows;
        public int ZRows;
    }
}
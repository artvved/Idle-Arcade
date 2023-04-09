using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StackData : ScriptableObject
    {
        public float Offset;
        public int XRows;
        public int YRows;
        public int ZRows;
    }
}
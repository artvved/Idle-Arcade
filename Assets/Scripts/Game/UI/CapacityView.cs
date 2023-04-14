using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class CapacityView : MonoBehaviour
    {
        public TextMeshProUGUI TextMeshProUGUI;
        
        public void SetText(int count,int max)
        {
            TextMeshProUGUI.text = $"{count}/{max}";
        }
        public void SetImage(float p)
        {
           
        }
    }
}
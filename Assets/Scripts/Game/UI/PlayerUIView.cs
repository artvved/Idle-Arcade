using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerUIView : MonoBehaviour
    {
        public TextMeshProUGUI TextMeshProUGUI;
        
        public void ShowHideMax(bool show)
        {
            if (show)
            {
                TextMeshProUGUI.text ="MAX";
            }
            else
            {
                TextMeshProUGUI.text ="";
            }

        }
        
    }
}
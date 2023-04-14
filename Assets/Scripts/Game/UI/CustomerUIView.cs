using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class CustomerUIView : MonoBehaviour
    {
        public TextMeshProUGUI TextMeshProUGUI;
        public GameObject Capacity;
        public Image SingleImage;
        
        public void SetText(int count,int max)
        {
            TextMeshProUGUI.text = $"{count}/{max}";
            Capacity.SetActive(true);
            SingleImage.gameObject.SetActive(false);
            
        }
        public void SetSingleImage(Sprite sprite)
        {
            Capacity.SetActive(false);
            SingleImage.gameObject.SetActive(true);
            SingleImage.sprite = sprite;
        }
    }
}
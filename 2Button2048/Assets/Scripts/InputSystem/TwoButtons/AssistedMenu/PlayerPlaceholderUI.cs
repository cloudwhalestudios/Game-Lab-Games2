using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class PlayerPlaceholderUI : MonoBehaviour
        {
            public TextMeshProUGUI nameText;
            public TextMeshProUGUI primary;
            public TextMeshProUGUI secondary;

            public void ShowKeys(bool show)
            {
                primary.gameObject.SetActive(show);
                secondary.gameObject.SetActive(show);
            }

            public void SetColor(Color color)
            {
                GetComponent<Image>().color = color;
            }

            internal void Init(string name, KeyCode primaryKey, KeyCode secondaryKey)
            {
                nameText.text = name;
                primary.text = primaryKey.ToString();
                secondary.text = secondaryKey.ToString();
            }
        }
    }
}
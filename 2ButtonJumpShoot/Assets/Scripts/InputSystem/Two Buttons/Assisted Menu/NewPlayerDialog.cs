using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class NewPlayerDialog : MonoBehaviour
        {
            public Text nameText;
            public Text primaryKeyText;
            public Text promptText;

            public string defaultText = "Waiting for secondary button...";

            public void OnEnable()
            {
                BasePlayerManager.NewPlayerBeingAdded += BasePlayerManager_NewPlayerBeingAdded;
                BasePlayerManager.NewPlayerKeyInUse += BasePlayerManager_NewPlayerKeyInUse;
                BasePlayerManager.NewPlayerAdded += BasePlayerManager_NewPlayerAdded;
            }

            private void BasePlayerManager_NewPlayerAdded(BasePlayer player)
            {
                HideDialog();
            }

            private void BasePlayerManager_NewPlayerKeyInUse(string message, BasePlayerManager.KeyEventSpecifier keySpecifier)
            {
                DisplayKeyInUse(keySpecifier.Key.ToString());
            }

            private void BasePlayerManager_NewPlayerBeingAdded(string name, BasePlayerManager.KeyEventSpecifier[] keySpecifiers)
            {
                ShowDialog(name, keySpecifiers[0].Key.ToString(), Color.black);
            }

            public void ShowDialog(string playerName, string primaryKey, Color color)
            {
                promptText.text = defaultText;
                nameText.text = playerName;
                nameText.color = color;
                primaryKeyText.text = primaryKey;

                gameObject.SetActive(true);
            }

            public void HideDialog()
            {
                gameObject.SetActive(false);
            }

            public void DisplayKeyInUse(string secondaryKey)
            {
                promptText.text = $"Button '{secondaryKey}' is already in use!\nPlease choose another...";
            }
        }
    }
}
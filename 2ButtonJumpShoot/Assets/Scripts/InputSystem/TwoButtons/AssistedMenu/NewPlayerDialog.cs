using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class NewPlayerDialog : MonoBehaviour
        {
            public GameObject DialogPanel;
            public TextMeshProUGUI nameText;
            public TextMeshProUGUI primaryKeyText;
            public TextMeshProUGUI promptText;

            public GameObject WaitingForPlayerInput;
            public GameObject PlayerUIPrefab;

            GameObject playerUI;

            public string defaultText = "Waiting for secondary button...";

            public void OnEnable()
            {
                BasePlayerManager.NewPlayerBeingAdded += BasePlayerManager_NewPlayerBeingAdded;
                BasePlayerManager.NewPlayerKeyInUse += BasePlayerManager_NewPlayerKeyInUse;
                BasePlayerManager.NewPlayerAdded += BasePlayerManager_NewPlayerAdded;
                BasePlayerManager.PlayerRemoved += BasePlayerManager_PlayerRemoved;

                HideDialog();
            }
            public void OnDisable()
            {
                BasePlayerManager.NewPlayerBeingAdded -= BasePlayerManager_NewPlayerBeingAdded;
                BasePlayerManager.NewPlayerKeyInUse -= BasePlayerManager_NewPlayerKeyInUse;
                BasePlayerManager.NewPlayerAdded -= BasePlayerManager_NewPlayerAdded;
                BasePlayerManager.PlayerRemoved -= BasePlayerManager_PlayerRemoved;
            }

            public void Start()
            {
                AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI);
                if (BasePlayerManager.Instance.PlayerCount > 0)
                {
                    AddPlayerPlaceholderUI(BasePlayerManager.Instance.GetPlayer());
                }
            }

            private void BasePlayerManager_PlayerRemoved(int newCount)
            {
                AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI_cancel);
                if (newCount == 0)
                {
                    WaitingForPlayerInput.SetActive(true);
                    if (playerUI != null)
                    {
                        DestroyImmediate(playerUI);
                    }
                }
            }

            private void BasePlayerManager_NewPlayerAdded(BasePlayer player)
            {
                HideDialog();

                AddPlayerPlaceholderUI(player);
                AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI_confirm);
            }

            void AddPlayerPlaceholderUI(BasePlayer player)
            {
                WaitingForPlayerInput.SetActive(false);

                playerUI = Instantiate(PlayerUIPrefab, WaitingForPlayerInput.transform.parent);
                var ui = playerUI.GetComponent<PlayerPlaceholderUI>();
                if (ui != null)
                {
                    ui.Init(player.Name, player.Keys[0], player.Keys[1]);
                }
            }

            private void BasePlayerManager_NewPlayerKeyInUse(string message, BasePlayerManager.KeyEventSpecifier keySpecifier)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.Alternate);
                DisplayKeyInUse(keySpecifier.Key.ToString());
            }

            private void BasePlayerManager_NewPlayerBeingAdded(string name, BasePlayerManager.KeyEventSpecifier[] keySpecifiers)
            {
                AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI_confirm);
                WaitingForPlayerInput.SetActive(false);
                ShowDialog(name, keySpecifiers[0].Specifier + ": " + keySpecifiers[0].Key.ToString(), Color.white);
            }

            public void ShowDialog(string playerName, string primaryKey, Color color)
            {
                promptText.text = defaultText;
                nameText.text = playerName;
                nameText.color = color;
                primaryKeyText.text = primaryKey;

                DialogPanel.SetActive(true);
            }

            public void HideDialog()
            {
                DialogPanel.SetActive(false);
            }

            public void DisplayKeyInUse(string secondaryKey)
            {
                promptText.text = $"Button '{secondaryKey}' is already in use!\nPlease choose another...";
            }
        }
    }
}
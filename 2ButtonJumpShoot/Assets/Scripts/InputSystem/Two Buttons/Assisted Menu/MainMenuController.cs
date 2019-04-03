using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class MainMenuController : BaseMenuController
        {
            [SerializeField, ReadOnly] private bool isReady = false;

            public void Start()
            {
                MenuManager.Instance.SetActiveMenu(this);
                itemSelectIndicator.gameObject.SetActive(false);
            }

            void OnEnable()
            {
                BasePlayerManager.NewPlayerAdded += PlayerManager_NewPlayerAdded;
                BasePlayerManager.PlayerRemoved += PlayerManager_PlayerWasRemoved;
            }

            void OnDisable()
            {
                BasePlayerManager.NewPlayerAdded -= PlayerManager_NewPlayerAdded;
                BasePlayerManager.PlayerRemoved -= PlayerManager_PlayerWasRemoved;
            }

            private void PlayerManager_NewPlayerAdded(BasePlayer player)
            {
                if (isReady) return;
                isReady = true;
                MenuManager.Instance.StartIndicating();
            }

            private void PlayerManager_PlayerWasRemoved(int total)
            {
                if (total != 0) return;
                isReady = false;
                MenuManager.Instance.StartIndicating(false);
            }
        }
    }
}
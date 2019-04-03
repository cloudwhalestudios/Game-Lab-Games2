using SgLib;
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
            public void Start()
            {
                MenuManager.Instance.SetActiveMenu(this);
                itemSelectIndicator.gameObject.SetActive(false);
            }

            void OnEnable()
            {
                PlayerManager.NewPlayerAdded += PlayerManager_NewPlayerAdded;
                PlayerManager.PlayerWasRemoved += PlayerManager_PlayerWasRemoved;
            }

            void OnDisable()
            {
                PlayerManager.NewPlayerAdded -= PlayerManager_NewPlayerAdded;
                PlayerManager.PlayerWasRemoved -= PlayerManager_PlayerWasRemoved;
            }

            private void PlayerManager_NewPlayerAdded(int total)
            {
                if (total != 1) return;
                MenuManager.Instance.StartIndicating();
            }

            private void PlayerManager_PlayerWasRemoved(int total)
            {
                if (total != 0) return;
                MenuManager.Instance.StartIndicating(false);
            }
        }
    }
}
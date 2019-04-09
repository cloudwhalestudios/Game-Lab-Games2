using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class GameMenuController : BaseMenuController
        {
            public void SetMenu(GameObject _menuContainer, GameObject _buttonParent)
            {
                menuContainer = _menuContainer;
                buttonParent = _buttonParent;
                MenuManager.Instance.SetMenuController(this);
            }
        }
    }
}
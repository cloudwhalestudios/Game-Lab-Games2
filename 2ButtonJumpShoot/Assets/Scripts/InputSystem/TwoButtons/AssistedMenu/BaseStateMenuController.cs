using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        [Serializable]
        public class StateMenu
        {
            public string name;
            public GameObject highlight;
            public RectTransform stateTimer;
            public BaseMenuController menuController;

            // To handle the special interaction for the library state menu in the launcher
            public UnityEvent selectEvent;
            public UnityEvent returnEvent;
        }

        public class BaseStateMenuController : MonoBehaviour
        {
            public enum Mode
            {
                Single,
                State
            }

            public Mode indicatorMode = Mode.Single;
            public RectTransform stateSelectTimer;

            [Header("Selection Behaviour")]
            public int startStateIndex;

            [Space]
            public List<StateMenu> stateMenus = new List<StateMenu>();
        }
    }
}

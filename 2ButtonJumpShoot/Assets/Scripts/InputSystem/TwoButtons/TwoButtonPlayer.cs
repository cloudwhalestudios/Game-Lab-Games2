using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class TwoButtonPlayer : BasePlayer
        {
            public TwoButtonPlayer(string name, KeyCode primaryKey, KeyCode secondaryKey, Transform parent = null)
                : base(name, parent)
            {
                SetupInputController(primaryKey, secondaryKey);
            }

            protected void SetupInputController(KeyCode primaryKey, KeyCode secondaryKey)
            {
                InputController = PGameObject.GetComponent<TwoButtonInputController>();
                if (InputController == null)
                {
                    InputController = PGameObject.AddComponent<TwoButtonInputController>();
                }

                InputController.SetControls(primaryKey, secondaryKey);

                Keys = new KeyCode[] { primaryKey, secondaryKey };
            }
        }
    }
}

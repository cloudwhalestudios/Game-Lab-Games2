using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class TwoButtonInputController : BaseInputController
        {
            public InputKeyEvent primary = new InputKeyEvent();
            public InputKeyEvent secondary = new InputKeyEvent();

            public override void SetControls(params KeyCode[] keys)
            {
                if (keys?.Length < 2)
                {
                    throw new System.ArgumentNullException(nameof(keys));
                }

                primary.Key = keys[0];
                secondary.Key = keys[1];

                inputKeyEvents.Add(primary);
                inputKeyEvents.Add(secondary);
            }
        }
    }
}
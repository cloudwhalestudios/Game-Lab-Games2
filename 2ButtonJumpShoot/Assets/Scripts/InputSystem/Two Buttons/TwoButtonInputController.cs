using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class TwoButtonInputController : BaseInputController
        {
            public InputKeyEvent primary;
            public InputKeyEvent secondary;

            public override void SetControls(params KeyCode[] keys)
            {
                if (keys?.Length < 2)
                {
                    throw new System.ArgumentNullException(nameof(keys));
                }

                primary = new InputKeyEvent(keys[0]);
                secondary = new InputKeyEvent(keys[1]);
            }
        }
    }
}
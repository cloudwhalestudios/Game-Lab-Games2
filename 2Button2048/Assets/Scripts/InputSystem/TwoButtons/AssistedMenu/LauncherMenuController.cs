using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class LauncherMenuController : BaseMenuController
        {
            public void Start()
            {
                ResetController();
            }

            public void ResetController()
            {
                if (itemSelectIndicator != null)
                {
                    itemSelectIndicator.gameObject.SetActive(false);
                }
                if (itemSelectTimer != null)
                {
                    itemSelectTimer.localScale = new Vector3(0, 1, 1);
                    itemSelectTimer.gameObject.SetActive(false);

                }
            }
        }
    }
}
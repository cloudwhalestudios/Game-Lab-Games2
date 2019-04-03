using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    public abstract class ActiveInputHandler : MonoBehaviour
    {
        BaseInputController controller;

        protected virtual void OnEnable()
        {
            controller = GetComponent<BaseInputController>();

            if (controller is TwoButtons.TwoButtonInputController twoButtonControls)
            {
                twoButtonControls.primary.InputEvent += TBPrimary_InputEvent;
                twoButtonControls.secondary.InputEvent += TBSecondary_InputEvent;
                //Debug.Log("Registerd two button handlers");
            }

            //Debug.Log("Finished attaching handles");
        }
        protected virtual void OnDisable()
        {
            if (controller is TwoButtons.TwoButtonInputController twoButtonControls)
            {
                twoButtonControls.primary.InputEvent -= TBPrimary_InputEvent;
                twoButtonControls.secondary.InputEvent -= TBSecondary_InputEvent;
            }
        }

        protected abstract void TBPrimary_InputEvent(KeyCode primaryKey);
        protected abstract void TBSecondary_InputEvent(KeyCode secondaryKey);
    }
}
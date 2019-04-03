using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AccessibilityInputSystem
{
    public abstract class BaseInputController : MonoBehaviour
    {
        [Serializable]
        public class InputKeyEvent
        {
            [SerializeField, ReadOnly] private KeyCode _key;
            public KeyCode Key { get => _key; set => _key = value; }
            public event Action<KeyCode> InputEvent;

            public void Invoke(KeyCode key = KeyCode.None)
            {
                if (key == KeyCode.None || key == Key)
                {
                    InputEvent?.Invoke(key);
                }
            }
        }

        [SerializeField] private int _pID = -1;
        protected int PID { get => _pID; private set => _pID = value; }
        protected static Dictionary<int, BaseInputController> inputControllers;

        public virtual void Init(int pID)
        {
            if (inputControllers == null)
            {
                inputControllers = new Dictionary<int, BaseInputController>();
            }
            else
            {
                // Check if already there
                var controller = inputControllers[pID];
                if (controller != null)
                {
                    Debug.LogError($"Input controller for player with the id {pID} already exists!");
                    return;
                }
            }

            // TODO check for existing keys to avoid double mapping
            if (inputControllers.ContainsKey(pID))
            {
                // Update controls for key
                inputControllers[pID] = this;
            }
            else
            {
                // Add controls for key
                inputControllers.Add(pID, this);
            }
        }
    }
}
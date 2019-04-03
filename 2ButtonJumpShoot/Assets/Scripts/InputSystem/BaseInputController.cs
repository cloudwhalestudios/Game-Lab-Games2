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

            public InputKeyEvent(KeyCode key)
            {
                Key = key;
            }

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

        public abstract void SetControls(params KeyCode[] keys);
    }
}
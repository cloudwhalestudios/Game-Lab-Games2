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
                //Debug.Log("Invoking " + key.ToString());
                if (key == KeyCode.None || key == Key)
                {
                    //Debug.Log("Input event is null: " + InputEvent == null);
                    InputEvent?.Invoke(key);
                }
            }
        }

        protected List<InputKeyEvent> inputKeyEvents = new List<InputKeyEvent>();

        protected virtual void Update()
        {
            foreach (var inputKeyEvent in inputKeyEvents)
            {
                //Debug.Log("Checking Input");
                if (Input.GetKeyDown(inputKeyEvent.Key))
                {
                    inputKeyEvent.Invoke();
                }
            }
        }

        public KeyCode GetKey(int index)
        {
            if (inputKeyEvents[index] != null)
                return inputKeyEvents[index].Key;

            return KeyCode.None;
        }

        public abstract void SetControls(params KeyCode[] keys);
    }
}
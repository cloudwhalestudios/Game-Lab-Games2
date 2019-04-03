using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    public abstract class BasePlayerManager : MonoBehaviour
    {
        public static event Action<int> NewPlayerAdded;
        public static event Action<int> PlayerWasRemoved;

        public static BasePlayerManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] public GameObject playerPrefab;

        [Header("Players")]
        [SerializeField, ReadOnly] private bool waitingForNextInput;
        [SerializeField, ReadOnly] private Dictionary<KeyCode, BasePlayer> playerKeyBindings;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        void OnDestroy()
        {
            if (Instance == this) { Instance = null; }
            StopAllCoroutines();
        }

        public static KeyCode GetKeyInput()
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    return kcode;
                }
            }
            return KeyCode.None;
        }
    }
}
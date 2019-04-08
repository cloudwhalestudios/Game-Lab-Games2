using PlayerPreferences;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGLIntegration;

[Serializable]
public class PlatformPreferences
{
    static PlatformPreferences current;
    static bool hasLoaded = false;

    [SerializeField] bool completedSetup;

    [SerializeField] KeyCode[] keys;

    [SerializeField] float menuProgressionTimer = 2f;

    public static PlatformPreferences Current
    {
        get
        {
            if (current != null)
                return current;

            current = PlayerPreferenceManager.Load<PlatformPreferences>();
            return current;
        }
        set
        {
            if (!hasLoaded)
            {
                Debug.Log("Setting up platform preferences: " + value + " (Json: " + JsonUtility.ToJson(value, true) + ")");
                current = value;
                hasLoaded = true;
            }
        }
    }

    public bool CompletedSetup { get => completedSetup; set { completedSetup = value; Save(); } }

    public KeyCode[] Keys { get => keys; set { keys = value; Save(); } }

    public float MenuProgressionTimer { get => menuProgressionTimer; set { menuProgressionTimer = value; Save(); } }


    public static void Save()
    {
        //PlayerPreferenceManager.Save(current);
    }
}

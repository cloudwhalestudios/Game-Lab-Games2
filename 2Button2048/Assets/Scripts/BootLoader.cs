using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGLIntegration;

public class BootLoader : MonoBehaviour
{
    private static bool lookedForPlatformPreferences = false;

    public static void LoadPlatformPlayer()
    {
        if (!lookedForPlatformPreferences)
        {
            lookedForPlatformPreferences = true;
            var jsonString = WebGLParameters.GetParameterJson();
            Debug.Log("Loaded parameters: " + JsonUtility.ToJson(jsonString,true));

            PlatformPreferences.Current = JsonUtility.FromJson<PlatformPreferences>(jsonString);

            if (PlatformPreferences.Current?.Keys != null && PlatformPreferences.Current?.Keys.Length > 0)
            {
                BasePlayerManager.Instance.AddPlayer(PlatformPreferences.Current.Keys);
            }
        }
    }
}

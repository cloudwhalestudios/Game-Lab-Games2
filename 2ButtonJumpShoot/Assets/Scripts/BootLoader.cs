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
            PlatformPreferences.Current = JsonUtility.FromJson<PlatformPreferences>(WebGLParameters.GetParameterJson());
            if (PlatformPreferences.Current?.Keys != null)
            {
                BasePlayerManager.Instance.AddPlayer(PlatformPreferences.Current.Keys);
            }
        }
    }
}

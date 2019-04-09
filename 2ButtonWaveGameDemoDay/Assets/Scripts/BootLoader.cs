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
            var jsonString = WebGLParameters.GetParameterJson();
            Debug.Log("Parameters in unity: " + jsonString);
            
        }
    }
}

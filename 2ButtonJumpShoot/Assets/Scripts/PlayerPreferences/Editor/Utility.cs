using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayerPreferences
{
    public class Utility : MonoBehaviour
    {
        public static event Action PlayerPreferenceCleared;

#if UNITY_EDITOR
        [MenuItem("Tools/Clear PlayerPrefs")]
        private static void NewMenuOption()
        {
            PlayerPrefs.DeleteAll();
            PlayerPreferenceCleared?.Invoke();
        }
#endif
    }
}
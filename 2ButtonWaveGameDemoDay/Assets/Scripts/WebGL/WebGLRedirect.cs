using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace WebGLIntegration
{
    public class WebGLRedirect
    {
        private static void Redirect(string url)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JSLib.Redirect(Config.BASE_URL + url + "/");
#endif
            Debug.Log($"Should open {Config.BASE_URL + url + "/"} right now");
        }

        public static void OpenLauncher()
        {
            Redirect(Config.LAUNCHER_URL);
        }

        public static void Refresh()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JSLib.Refresh();
#endif
        }
    }
}